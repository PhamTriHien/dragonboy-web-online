using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

string portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portEnv) && int.TryParse(portEnv, out int envPort))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{envPort}");
}
else if (args.Length == 0)
{
    builder.WebHost.UseUrls("http://0.0.0.0:5080");
}

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

app.UseResponseCompression();
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(20)
});

string sharedRoot = ResolveSharedRoot(app.Environment.ContentRootPath);
string assetsDir = Path.Combine(sharedRoot, "Assets");
string backgroundsDir = Path.Combine(sharedRoot, "Backgrounds");
string customLogoPath = Path.Combine(sharedRoot, "custom_logo.png");

Console.WriteLine($"[DragonBoy Web Server] Shared Root: {sharedRoot}");
Console.WriteLine($"[DragonBoy Web Server] Assets Dir : {assetsDir}");

byte[] startupPackBytes = BuildStartupPack(assetsDir, customLogoPath);
Console.WriteLine($"[DragonBoy Web Server] Startup Pack built: {startupPackBytes.Length / 1024} KB");

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".ttf"] = "font/ttf";
contentTypeProvider.Mappings[".otf"] = "font/otf";
contentTypeProvider.Mappings[".wasm"] = "application/wasm";
contentTypeProvider.Mappings[".dat"] = "application/octet-stream";

if (Directory.Exists(assetsDir))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(assetsDir),
        RequestPath = "/game-assets",
        ContentTypeProvider = contentTypeProvider,
        ServeUnknownFileTypes = true,
        DefaultContentType = "application/octet-stream",
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=604800";
        }
    });
}

if (Directory.Exists(backgroundsDir))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(backgroundsDir),
        RequestPath = "/backgrounds",
        ContentTypeProvider = contentTypeProvider,
        ServeUnknownFileTypes = true,
        DefaultContentType = "image/png",
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers["Cache-Control"] = "public, max-age=604800";
        }
    });
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider,
    ServeUnknownFileTypes = true
});

app.MapGet("/api/startup-pack", () => Results.Bytes(startupPackBytes, "application/octet-stream"));

HttpClient proxyHttp = new HttpClient { Timeout = TimeSpan.FromSeconds(25) };
proxyHttp.DefaultRequestHeaders.UserAgent.ParseAdd("DragonBoy-WebProxy/1.0");

app.MapGet("/api/proxy", async (HttpContext context) =>
{
    string rawUrl = context.Request.Query["url"].ToString();
    if (string.IsNullOrWhiteSpace(rawUrl) || !Uri.TryCreate(rawUrl, UriKind.Absolute, out Uri targetUri))
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid url parameter.");
        return;
    }
    if (targetUri.Scheme != Uri.UriSchemeHttp && targetUri.Scheme != Uri.UriSchemeHttps)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Unsupported scheme.");
        return;
    }

    try
    {
        using var resp = await proxyHttp.GetAsync(targetUri, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.StatusCode = (int)resp.StatusCode;
        if (resp.Content.Headers.ContentType != null)
        {
            context.Response.ContentType = resp.Content.Headers.ContentType.ToString();
        }
        if (resp.Content.Headers.ContentLength.HasValue)
        {
            context.Response.ContentLength = resp.Content.Headers.ContentLength.Value;
        }
        await resp.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 502;
        await context.Response.WriteAsync("Proxy error: " + ex.Message);
    }
});

app.Map("/ws/tcp", async (HttpContext context) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("WebSocket connection required.");
        return;
    }

    string host = context.Request.Query["host"].ToString().Trim();
    string portStr = context.Request.Query["port"].ToString().Trim();
    if (string.IsNullOrEmpty(host) || !int.TryParse(portStr, out int port) || port <= 0 || port > 65535)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid host or port.");
        return;
    }

    using WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
    using TcpClient tcp = new TcpClient();
    tcp.NoDelay = true;
    tcp.SendBufferSize = 65536;
    tcp.ReceiveBufferSize = 131072;

    try
    {
        IPAddress targetIp = null;
        if (!IPAddress.TryParse(host, out targetIp))
        {
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(host, context.RequestAborted);
            targetIp = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                       ?? addresses.FirstOrDefault();
        }

        if (targetIp == null)
        {
            await ws.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "DNS resolution failed", CancellationToken.None);
            return;
        }

        using var connectCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
        connectCts.CancelAfter(TimeSpan.FromSeconds(10));
        await tcp.ConnectAsync(targetIp, port, connectCts.Token);

        Console.WriteLine($"[WS->TCP] Connected {context.Connection.RemoteIpAddress} -> {host} ({targetIp}):{port}");

        using NetworkStream netStream = tcp.GetStream();
        using var relayCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);

        System.Threading.Tasks.Task wsToTcp = PumpWebSocketToTcpAsync(ws, netStream, relayCts.Token);
        System.Threading.Tasks.Task tcpToWs = PumpTcpToWebSocketAsync(netStream, ws, relayCts.Token);

        await System.Threading.Tasks.Task.WhenAny(wsToTcp, tcpToWs);
        relayCts.Cancel();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[WS->TCP] Error ({host}:{port}): {ex.Message}");
    }
    finally
    {
        if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
        {
            try
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            }
            catch { }
        }
    }
});

app.MapFallbackToFile("index.html");

app.Run();

static async System.Threading.Tasks.Task PumpWebSocketToTcpAsync(WebSocket ws, NetworkStream netStream, CancellationToken ct)
{
    byte[] buf = ArrayPool<byte>.Shared.Rent(65536);
    try
    {
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buf), ct);
            if (result.MessageType == WebSocketMessageType.Close || result.Count <= 0)
            {
                break;
            }
            await netStream.WriteAsync(buf.AsMemory(0, result.Count), ct);
            await netStream.FlushAsync(ct);
        }
    }
    catch { }
    finally
    {
        ArrayPool<byte>.Shared.Return(buf);
    }
}

static async System.Threading.Tasks.Task PumpTcpToWebSocketAsync(NetworkStream netStream, WebSocket ws, CancellationToken ct)
{
    byte[] buf = ArrayPool<byte>.Shared.Rent(65536);
    try
    {
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            int read = await netStream.ReadAsync(buf.AsMemory(0, buf.Length), ct);
            if (read <= 0)
            {
                break;
            }
            await ws.SendAsync(new ArraySegment<byte>(buf, 0, read), WebSocketMessageType.Binary, true, ct);
        }
    }
    catch { }
    finally
    {
        ArrayPool<byte>.Shared.Return(buf);
    }
}

static string ResolveSharedRoot(string contentRoot)
{
    string[] candidates =
    {
        Path.GetFullPath(Path.Combine(contentRoot, "DragonBoy_Shared")),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "DragonBoy_Shared")),
        Path.GetFullPath(Path.Combine(contentRoot, "..", "..", "DragonBoy_Shared")),
        Path.GetFullPath(Path.Combine(contentRoot, "..", "DragonBoy_Shared")),
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "DragonBoy_Shared")),
        @"C:\ModNRO\DragonBoy_Shared"
    };

    foreach (string c in candidates)
    {
        if (Directory.Exists(c) && Directory.Exists(Path.Combine(c, "Assets")))
        {
            return c;
        }
    }
    return candidates[0];
}

static byte[] BuildStartupPack(string assetsDir, string customLogoPath)
{
    var pngDimensions = new List<(string relPath, short w, short h)>(4096);
    var bundledFiles = new List<(string relPath, byte[] bytes)>(64);

    if (Directory.Exists(assetsDir))
    {
        foreach (string file in Directory.EnumerateFiles(assetsDir, "*.png", SearchOption.AllDirectories))
        {
            string rel = Path.GetRelativePath(assetsDir, file).Replace('\\', '/');
            if (TryReadPngDimensions(file, out int w, out int h))
            {
                pngDimensions.Add((rel, (short)System.Math.Clamp(w, 1, short.MaxValue), (short)System.Math.Clamp(h, 1, short.MaxValue)));
            }
        }

        string dataDir = Path.Combine(assetsDir, "data");
        if (Directory.Exists(dataDir))
        {
            foreach (string file in Directory.EnumerateFiles(dataDir, "*", SearchOption.AllDirectories))
            {
                string rel = Path.GetRelativePath(assetsDir, file).Replace('\\', '/');
                bundledFiles.Add((rel, File.ReadAllBytes(file)));
            }
        }
    }

    if (File.Exists(customLogoPath))
    {
        bundledFiles.Add(("custom_logo.png", File.ReadAllBytes(customLogoPath)));
    }

    using var ms = new MemoryStream(1024 * 512);
    using var bw = new BinaryWriter(ms);

    bw.Write(pngDimensions.Count);
    foreach (var item in pngDimensions)
    {
        bw.Write(item.relPath);
        bw.Write(item.w);
        bw.Write(item.h);
    }

    bw.Write(bundledFiles.Count);
    foreach (var f in bundledFiles)
    {
        bw.Write(f.relPath);
        bw.Write(f.bytes.Length);
        bw.Write(f.bytes);
    }

    bw.Flush();
    return ms.ToArray();
}

static bool TryReadPngDimensions(string filePath, out int width, out int height)
{
    width = 1;
    height = 1;
    try
    {
        Span<byte> header = stackalloc byte[24];
        using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        if (fs.Read(header) < 24) return false;
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
        {
            width = (header[16] << 24) | (header[17] << 16) | (header[18] << 8) | header[19];
            height = (header[20] << 24) | (header[21] << 16) | (header[22] << 8) | header[23];
            return width > 0 && height > 0;
        }
    }
    catch { }
    return false;
}
