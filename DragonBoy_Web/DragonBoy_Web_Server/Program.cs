using System.Buffers;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.ResponseCompression;
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
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "application/wasm",
        "application/octet-stream",
        "font/ttf",
        "font/otf"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

var app = builder.Build();

app.UseResponseCompression();
app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(15)
});

string sharedRoot = ResolveSharedRoot(app.Environment.ContentRootPath);
string assetsDir = Path.Combine(sharedRoot, "Assets");
string backgroundsDir = Path.Combine(sharedRoot, "Backgrounds");
string customLogoPath = Path.Combine(sharedRoot, "custom_logo.png");

Console.WriteLine($"[DragonBoy Web Server .NET 10] Shared Root: {sharedRoot}");
Console.WriteLine($"[DragonBoy Web Server .NET 10] Assets Dir : {assetsDir}");

byte[] startupPackBytes = BuildStartupPack(assetsDir, customLogoPath);
byte[] startupPackBr = CompressBrotli(startupPackBytes);
byte[] startupPackGz = CompressGzip(startupPackBytes);
string startupPackETag = $"\"sp-{Convert.ToHexString(SHA256.HashData(startupPackBytes))[..16]}\"";
Console.WriteLine($"[DragonBoy Web Server .NET 10] Startup Pack: Raw={startupPackBytes.Length / 1024} KB | Br={startupPackBr.Length / 1024} KB | Gz={startupPackGz.Length / 1024} KB | ETag={startupPackETag}");

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
    ServeUnknownFileTypes = true,
    OnPrepareResponse = ctx =>
    {
        string name = ctx.File.Name;
        if (name.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("blazor.boot.json", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("dragonboy.js", StringComparison.OrdinalIgnoreCase))
        {
            ctx.Context.Response.Headers["Cache-Control"] = "no-cache";
        }
    }
});

app.MapMethods("/healthz", new[] { "GET", "HEAD" }, (HttpContext ctx) =>
{
    ctx.Response.Headers["Cache-Control"] = "no-store, no-cache";
    return Results.Text("ok", "text/plain");
});

app.MapMethods("/api/startup-pack", new[] { "GET", "HEAD" }, async (HttpContext ctx) =>
{
    ctx.Response.Headers["ETag"] = startupPackETag;
    ctx.Response.Headers["Vary"] = "Accept-Encoding";
    ctx.Response.Headers["Cache-Control"] = "public, max-age=300, stale-while-revalidate=86400";

    string inm = ctx.Request.Headers.IfNoneMatch.ToString();
    if (!string.IsNullOrEmpty(inm) && (inm.Contains(startupPackETag, StringComparison.Ordinal) || inm.Trim() == "*"))
    {
        ctx.Response.StatusCode = StatusCodes.Status304NotModified;
        return;
    }

    string acceptEnc = ctx.Request.Headers.AcceptEncoding.ToString();
    byte[] payload = startupPackBytes;
    if (acceptEnc.Contains("br", StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.Headers["Content-Encoding"] = "br";
        payload = startupPackBr;
    }
    else if (acceptEnc.Contains("gzip", StringComparison.OrdinalIgnoreCase))
    {
        ctx.Response.Headers["Content-Encoding"] = "gzip";
        payload = startupPackGz;
    }

    ctx.Response.ContentType = "application/octet-stream";
    ctx.Response.ContentLength = payload.Length;
    if (!HttpMethods.IsHead(ctx.Request.Method))
    {
        await ctx.Response.Body.WriteAsync(payload, ctx.RequestAborted);
    }
});

var dnsCache = new ConcurrentDictionary<string, (IPAddress[] addresses, long expireTick)>(StringComparer.OrdinalIgnoreCase);
const long DnsCacheTtlMs = 300_000;

async System.Threading.Tasks.ValueTask<IPAddress[]> ResolveHostCachedAsync(string hostName, CancellationToken ct)
{
    long now = Environment.TickCount64;
    if (dnsCache.TryGetValue(hostName, out var entry) && now < entry.expireTick && entry.addresses.Length > 0)
    {
        return entry.addresses;
    }
    IPAddress[] resolved = await Dns.GetHostAddressesAsync(hostName, ct);
    if (resolved != null && resolved.Length > 0)
    {
        dnsCache[hostName] = (resolved, now + DnsCacheTtlMs);
    }
    return resolved ?? Array.Empty<IPAddress>();
}

HttpClient proxyHttp = new HttpClient(new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
    ConnectTimeout = TimeSpan.FromSeconds(10),
    AutomaticDecompression = DecompressionMethods.None
})
{
    Timeout = TimeSpan.FromSeconds(25)
};
proxyHttp.DefaultRequestHeaders.UserAgent.ParseAdd("DragonBoy-WebProxy/2.2");

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
        IPAddress[] hostIps = await ResolveHostCachedAsync(targetUri.DnsSafeHost, context.RequestAborted);
        if (hostIps.Length == 0 || hostIps.Any(IsBlockedInternalIp))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden target host.");
            return;
        }

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

app.MapPost("/api/proxy", async (HttpContext context) =>
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
    if (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > 2 * 1024 * 1024)
    {
        context.Response.StatusCode = 413;
        await context.Response.WriteAsync("Payload too large.");
        return;
    }

    try
    {
        IPAddress[] hostIps = await ResolveHostCachedAsync(targetUri.DnsSafeHost, context.RequestAborted);
        if (hostIps.Length == 0 || hostIps.Any(IsBlockedInternalIp))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync("Forbidden target host.");
            return;
        }

        using var bodyStream = new MemoryStream();
        byte[] readBuf = ArrayPool<byte>.Shared.Rent(16384);
        try
        {
            int totalBytes = 0;
            while (true)
            {
                int n = await context.Request.Body.ReadAsync(readBuf.AsMemory(0, readBuf.Length), context.RequestAborted);
                if (n <= 0) break;
                totalBytes += n;
                if (totalBytes > 2 * 1024 * 1024)
                {
                    context.Response.StatusCode = 413;
                    await context.Response.WriteAsync("Payload too large.");
                    return;
                }
                bodyStream.Write(readBuf, 0, n);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(readBuf);
        }

        using var upstreamReq = new HttpRequestMessage(HttpMethod.Post, targetUri)
        {
            Content = new ByteArrayContent(bodyStream.ToArray())
        };
        if (!string.IsNullOrEmpty(context.Request.ContentType) &&
            System.Net.Http.Headers.MediaTypeHeaderValue.TryParse(context.Request.ContentType, out var parsedMediaType))
        {
            upstreamReq.Content.Headers.ContentType = parsedMediaType;
        }

        using var resp = await proxyHttp.SendAsync(upstreamReq, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
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
    string proxyRaw = context.Request.Query["proxy"].ToString().Trim();
    if (string.IsNullOrEmpty(host) || !int.TryParse(portStr, out int port) || port <= 0 || port > 65535)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Invalid host or port.");
        return;
    }

    using WebSocket ws = await context.WebSockets.AcceptWebSocketAsync();
    TcpClient tcp = null;

    try
    {
        IPAddress targetIp = null;
        if (IPAddress.TryParse(host, out IPAddress literalTargetIp))
        {
            targetIp = literalTargetIp;
            if (IsBlockedInternalIp(targetIp))
            {
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Target address not allowed", CancellationToken.None);
                return;
            }
        }
        else
        {
            IPAddress[] addresses = await ResolveHostCachedAsync(host, context.RequestAborted);
            if (addresses.Any(IsBlockedInternalIp))
            {
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Target address not allowed", CancellationToken.None);
                return;
            }
            targetIp = addresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                       ?? addresses.FirstOrDefault();
        }

        NetworkStream netStream;
        if (!string.IsNullOrEmpty(proxyRaw))
        {
            if (!TryParseProxySpec(proxyRaw, out bool isHttpProxy, out string proxyHost, out int proxyPort, out string proxyUser, out string proxyPass))
            {
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Invalid proxy format", CancellationToken.None);
                return;
            }

            IPAddress proxyIp = null;
            if (!IPAddress.TryParse(proxyHost, out proxyIp))
            {
                IPAddress[] proxyAddresses = await ResolveHostCachedAsync(proxyHost, context.RequestAborted);
                if (proxyAddresses.Any(IsBlockedInternalIp))
                {
                    await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Proxy address not allowed", CancellationToken.None);
                    return;
                }
                proxyIp = proxyAddresses.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                          ?? proxyAddresses.FirstOrDefault();
            }

            if (proxyIp == null || IsBlockedInternalIp(proxyIp))
            {
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Proxy address not allowed", CancellationToken.None);
                return;
            }

            tcp = new TcpClient(proxyIp.AddressFamily);
            ConfigureLowLatencyTcpSocket(tcp);
            using (var connectCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
            {
                connectCts.CancelAfter(TimeSpan.FromSeconds(10));
                await tcp.ConnectAsync(proxyIp, proxyPort, connectCts.Token);
            }
            ConfigureLowLatencyTcpSocket(tcp);
            netStream = tcp.GetStream();

            using (var handshakeCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
            {
                handshakeCts.CancelAfter(TimeSpan.FromSeconds(12));
                string tunnelHost = (targetIp != null) ? targetIp.ToString() : host;
                if (isHttpProxy)
                {
                    await HandshakeHttpConnectAsync(netStream, tunnelHost, port, proxyUser, proxyPass, handshakeCts.Token);
                }
                else
                {
                    await HandshakeSocks5Async(netStream, tunnelHost, port, proxyUser, proxyPass, handshakeCts.Token);
                }
            }

            Console.WriteLine($"[WS->TCP Proxy] Connected {context.Connection.RemoteIpAddress} via {proxyHost}:{proxyPort} -> {host}:{port}");
        }
        else
        {
            if (targetIp == null || IsBlockedInternalIp(targetIp))
            {
                await ws.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Target address not allowed", CancellationToken.None);
                return;
            }

            tcp = new TcpClient(targetIp.AddressFamily);
            ConfigureLowLatencyTcpSocket(tcp);
            using (var connectCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted))
            {
                connectCts.CancelAfter(TimeSpan.FromSeconds(10));
                await tcp.ConnectAsync(targetIp, port, connectCts.Token);
            }
            ConfigureLowLatencyTcpSocket(tcp);
            netStream = tcp.GetStream();

            Console.WriteLine($"[WS->TCP] Connected {context.Connection.RemoteIpAddress} -> {host} ({targetIp}):{port}");
        }

        using (netStream)
        {
            using var relayCts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
            System.Threading.Tasks.Task wsToTcp = PumpWebSocketToTcpAsync(ws, netStream, relayCts.Token);
            System.Threading.Tasks.Task tcpToWs = PumpTcpToWebSocketAsync(netStream, ws, relayCts.Token);

            await System.Threading.Tasks.Task.WhenAny(wsToTcp, tcpToWs);
            relayCts.Cancel();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[WS->TCP] Error ({host}:{port}): {ex.Message}");
    }
    finally
    {
        if (tcp != null)
        {
            try { tcp.Dispose(); } catch { }
        }
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

static byte[] CompressBrotli(byte[] raw)
{
    using var ms = new MemoryStream(raw.Length / 3);
    using (var br = new BrotliStream(ms, CompressionLevel.SmallestSize, leaveOpen: true))
    {
        br.Write(raw, 0, raw.Length);
    }
    return ms.ToArray();
}

static byte[] CompressGzip(byte[] raw)
{
    using var ms = new MemoryStream(raw.Length / 2);
    using (var gz = new GZipStream(ms, CompressionLevel.SmallestSize, leaveOpen: true))
    {
        gz.Write(raw, 0, raw.Length);
    }
    return ms.ToArray();
}

static void ConfigureLowLatencyTcpSocket(TcpClient tcp)
{
    tcp.NoDelay = true;
    tcp.SendBufferSize = 65536;
    tcp.ReceiveBufferSize = 131072;
    try
    {
        Socket sock = tcp.Client;
        sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveTime, 15);
        sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveInterval, 5);
        sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.TcpKeepAliveRetryCount, 3);
    }
    catch { }
}

/// <summary>
/// Kiểm tra xem địa chỉ IP có thuộc dải Loopback, mạng nội bộ (Private) hoặc Link-Local cần chặn để chống SSRF hay không.
/// </summary>
static bool IsBlockedInternalIp(IPAddress ip)
{
    if (ip == null) return true;
    if (IPAddress.IsLoopback(ip)) return true;
    if (ip.IsIPv4MappedToIPv6)
    {
        ip = ip.MapToIPv4();
    }
    if (ip.AddressFamily == AddressFamily.InterNetworkV6)
    {
        return ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.Equals(IPAddress.IPv6None) || ip.Equals(IPAddress.IPv6Any);
    }
    if (ip.AddressFamily == AddressFamily.InterNetwork)
    {
        byte[] b = ip.GetAddressBytes();
        if (b[0] == 0 || b[0] == 10 || b[0] == 127) return true;
        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
        if (b[0] == 192 && b[1] == 168) return true;
        if (b[0] == 169 && b[1] == 254) return true;
    }
    return false;
}

/// <summary>
/// Chuyển tiếp luồng gói tin nhị phân từ WebSocket của trình duyệt sang kết nối TCP của máy chủ game.
/// </summary>
static async System.Threading.Tasks.Task PumpWebSocketToTcpAsync(WebSocket ws, NetworkStream netStream, CancellationToken ct)
{
    byte[] buf = ArrayPool<byte>.Shared.Rent(65536);
    try
    {
        while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            ValueWebSocketReceiveResult result = await ws.ReceiveAsync(buf.AsMemory(), ct);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
            if (result.Count > 0)
            {
                await netStream.WriteAsync(buf.AsMemory(0, result.Count), ct);
            }
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
            int totalRead = await netStream.ReadAsync(buf.AsMemory(0, buf.Length), ct);
            if (totalRead <= 0)
            {
                break;
            }
            while (totalRead < buf.Length && netStream.DataAvailable)
            {
                int more = netStream.Read(buf, totalRead, buf.Length - totalRead);
                if (more <= 0) break;
                totalRead += more;
            }
            await ws.SendAsync(buf.AsMemory(0, totalRead), WebSocketMessageType.Binary, endOfMessage: true, ct);
        }
    }
    catch { }
    finally
    {
        ArrayPool<byte>.Shared.Return(buf);
    }
}

/// <summary>
/// Xác định đường dẫn thư mục gốc DragonBoy_Shared chứa Assets và Backgrounds.
/// </summary>
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

/// <summary>
/// Đóng gói bảng kích thước ảnh PNG và các tệp dữ liệu nhị phân cốt lõi vào gói khởi động (/api/startup-pack).
/// </summary>
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

        string[] binaryDirs =
        {
            Path.Combine(assetsDir, "data"),
            Path.Combine(assetsDir, "mymap"),
            Path.Combine(assetsDir, "myfont"),
            Path.Combine(assetsDir, "ios_fonts"),
            Path.Combine(assetsDir, "x4", "effectdata"),
            Path.Combine(assetsDir, "x3", "effectdata"),
            Path.Combine(assetsDir, "x2", "effectdata"),
            Path.Combine(assetsDir, "x1", "effectdata")
        };

        foreach (string dirPath in binaryDirs)
        {
            if (!Directory.Exists(dirPath)) continue;
            foreach (string file in Directory.EnumerateFiles(dirPath, "*", SearchOption.AllDirectories))
            {
                string ext = Path.GetExtension(file);
                if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".meta", StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".bytes", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string rel = Path.GetRelativePath(assetsDir, file).Replace('\\', '/');
                bundledFiles.Add((rel, File.ReadAllBytes(file)));
            }
        }

        string loginCodesFile = Path.Combine(assetsDir, "login_codes_generated.txt");
        if (File.Exists(loginCodesFile))
        {
            bundledFiles.Add(("login_codes_generated.txt", File.ReadAllBytes(loginCodesFile)));
        }

        string infoFile = Path.Combine(assetsDir, "info");
        if (File.Exists(infoFile))
        {
            bundledFiles.Add(("info", File.ReadAllBytes(infoFile)));
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

/// <summary>
/// Đọc nhanh chiều rộng và chiều cao từ header IHDR 24-byte của tệp PNG.
/// </summary>
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

static bool TryParseProxySpec(string raw, out bool isHttp, out string host, out int port, out string user, out string pass)
{
    isHttp = false;
    host = string.Empty;
    port = 0;
    user = string.Empty;
    pass = string.Empty;

    if (string.IsNullOrEmpty(raw)) return false;
    string s = raw.Trim();
    if (s.Length == 0) return false;

    if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
    {
        isHttp = true;
        s = s.Substring(7);
    }
    else if (s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
    {
        isHttp = true;
        s = s.Substring(8);
    }
    else if (s.StartsWith("socks5://", StringComparison.OrdinalIgnoreCase))
    {
        s = s.Substring(9);
    }
    else if (s.StartsWith("socks://", StringComparison.OrdinalIgnoreCase))
    {
        s = s.Substring(8);
    }

    if (s.Contains('@'))
    {
        int atIdx = s.LastIndexOf('@');
        string authPart = s.Substring(0, atIdx);
        string hostPart = s.Substring(atIdx + 1);
        int colonAuth = authPart.IndexOf(':');
        if (colonAuth >= 0)
        {
            user = authPart.Substring(0, colonAuth);
            pass = authPart.Substring(colonAuth + 1);
        }
        else
        {
            user = authPart;
        }
        int colonHost = hostPart.LastIndexOf(':');
        if (colonHost <= 0) return false;
        host = hostPart.Substring(0, colonHost).Trim();
        if (!int.TryParse(hostPart.Substring(colonHost + 1).Trim(), out port) || port <= 0 || port > 65535) return false;
        return host.Length > 0;
    }

    string[] parts = s.Split(':');
    if (parts.Length < 2) return false;
    host = parts[0].Trim();
    if (host.Length == 0) return false;
    if (!int.TryParse(parts[1].Trim(), out port) || port <= 0 || port > 65535) return false;

    if (parts.Length >= 4)
    {
        user = parts[2];
        pass = string.Join(":", parts, 3, parts.Length - 3);
    }
    else if (parts.Length == 3)
    {
        user = parts[2];
    }
    return true;
}

static async System.Threading.Tasks.Task ReadExactStreamAsync(Stream stream, byte[] buf, int offset, int count, CancellationToken ct)
{
    int total = 0;
    while (total < count)
    {
        int n = await stream.ReadAsync(buf.AsMemory(offset + total, count - total), ct);
        if (n <= 0)
        {
            throw new EndOfStreamException("Proxy closed connection during handshake.");
        }
        total += n;
    }
}

static async System.Threading.Tasks.Task HandshakeHttpConnectAsync(NetworkStream stream, string targetHost, int targetPort, string user, string pass, CancellationToken ct)
{
    StringBuilder sb = new StringBuilder();
    sb.Append("CONNECT ").Append(targetHost).Append(':').Append(targetPort).Append(" HTTP/1.1\r\n");
    sb.Append("Host: ").Append(targetHost).Append(':').Append(targetPort).Append("\r\n");
    if (!string.IsNullOrEmpty(user))
    {
        string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + (pass ?? string.Empty)));
        sb.Append("Proxy-Authorization: Basic ").Append(token).Append("\r\n");
    }
    sb.Append("Proxy-Connection: Keep-Alive\r\n\r\n");

    byte[] reqBytes = Encoding.ASCII.GetBytes(sb.ToString());
    await stream.WriteAsync(reqBytes.AsMemory(0, reqBytes.Length), ct);
    await stream.FlushAsync(ct);

    StringBuilder headerSb = new StringBuilder();
    byte[] oneByte = new byte[1];
    int matched = 0;
    int readCount = 0;
    while (readCount < 8192)
    {
        int n = await stream.ReadAsync(oneByte.AsMemory(0, 1), ct);
        if (n <= 0)
        {
            throw new EndOfStreamException("HTTP proxy closed connection before response.");
        }
        readCount++;
        char ch = (char)oneByte[0];
        headerSb.Append(ch);
        if ((matched == 0 && ch == '\r') ||
            (matched == 1 && ch == '\n') ||
            (matched == 2 && ch == '\r') ||
            (matched == 3 && ch == '\n'))
        {
            matched++;
            if (matched == 4) break;
        }
        else
        {
            matched = (ch == '\r') ? 1 : 0;
        }
    }

    string resp = headerSb.ToString();
    int firstLineEnd = resp.IndexOf('\r');
    string statusLine = (firstLineEnd > 0) ? resp.Substring(0, firstLineEnd) : resp;
    if (!statusLine.Contains(" 200", StringComparison.Ordinal))
    {
        throw new IOException("HTTP proxy rejected CONNECT: " + statusLine);
    }
}

static async System.Threading.Tasks.Task HandshakeSocks5Async(NetworkStream stream, string targetHost, int targetPort, string user, string pass, CancellationToken ct)
{
    bool hasAuth = !string.IsNullOrEmpty(user);
    byte[] greeting = hasAuth
        ? new byte[] { 0x05, 0x02, 0x00, 0x02 }
        : new byte[] { 0x05, 0x01, 0x00 };
    await stream.WriteAsync(greeting.AsMemory(0, greeting.Length), ct);
    await stream.FlushAsync(ct);

    byte[] methodResp = new byte[2];
    await ReadExactStreamAsync(stream, methodResp, 0, 2, ct);
    if (methodResp[0] != 0x05 || methodResp[1] == 0xFF)
    {
        throw new IOException("SOCKS5 proxy rejected authentication methods.");
    }

    if (methodResp[1] == 0x02)
    {
        byte[] uBytes = Encoding.UTF8.GetBytes(user ?? string.Empty);
        byte[] pBytes = Encoding.UTF8.GetBytes(pass ?? string.Empty);
        if (uBytes.Length > 255 || pBytes.Length > 255)
        {
            throw new IOException("SOCKS5 credentials exceed 255 bytes.");
        }
        byte[] authPacket = new byte[3 + uBytes.Length + pBytes.Length];
        authPacket[0] = 0x01;
        authPacket[1] = (byte)uBytes.Length;
        Buffer.BlockCopy(uBytes, 0, authPacket, 2, uBytes.Length);
        authPacket[2 + uBytes.Length] = (byte)pBytes.Length;
        Buffer.BlockCopy(pBytes, 0, authPacket, 3 + uBytes.Length, pBytes.Length);
        await stream.WriteAsync(authPacket.AsMemory(0, authPacket.Length), ct);
        await stream.FlushAsync(ct);

        byte[] authResp = new byte[2];
        await ReadExactStreamAsync(stream, authResp, 0, 2, ct);
        if (authResp[1] != 0x00)
        {
            throw new IOException("SOCKS5 authentication failed.");
        }
    }
    else if (methodResp[1] != 0x00)
    {
        throw new IOException("SOCKS5 unsupported auth method: " + methodResp[1]);
    }

    byte[] connectReq;
    if (IPAddress.TryParse(targetHost, out IPAddress parsedTargetIp))
    {
        byte[] addrBytes = parsedTargetIp.GetAddressBytes();
        byte atyp = (parsedTargetIp.AddressFamily == AddressFamily.InterNetworkV6) ? (byte)0x04 : (byte)0x01;
        connectReq = new byte[4 + addrBytes.Length + 2];
        connectReq[0] = 0x05;
        connectReq[1] = 0x01;
        connectReq[2] = 0x00;
        connectReq[3] = atyp;
        Buffer.BlockCopy(addrBytes, 0, connectReq, 4, addrBytes.Length);
        connectReq[4 + addrBytes.Length] = (byte)((targetPort >> 8) & 0xFF);
        connectReq[5 + addrBytes.Length] = (byte)(targetPort & 0xFF);
    }
    else
    {
        byte[] domainBytes = Encoding.ASCII.GetBytes(targetHost);
        if (domainBytes.Length > 255)
        {
            throw new IOException("Target domain exceeds SOCKS5 limit.");
        }
        connectReq = new byte[5 + domainBytes.Length + 2];
        connectReq[0] = 0x05;
        connectReq[1] = 0x01;
        connectReq[2] = 0x00;
        connectReq[3] = 0x03;
        connectReq[4] = (byte)domainBytes.Length;
        Buffer.BlockCopy(domainBytes, 0, connectReq, 5, domainBytes.Length);
        connectReq[5 + domainBytes.Length] = (byte)((targetPort >> 8) & 0xFF);
        connectReq[6 + domainBytes.Length] = (byte)(targetPort & 0xFF);
    }

    await stream.WriteAsync(connectReq.AsMemory(0, connectReq.Length), ct);
    await stream.FlushAsync(ct);

    byte[] repHead = new byte[4];
    await ReadExactStreamAsync(stream, repHead, 0, 4, ct);
    if (repHead[0] != 0x05 || repHead[1] != 0x00)
    {
        throw new IOException("SOCKS5 connect failed with code " + repHead[1]);
    }

    int remBytes;
    if (repHead[3] == 0x01)
    {
        remBytes = 6;
    }
    else if (repHead[3] == 0x04)
    {
        remBytes = 18;
    }
    else if (repHead[3] == 0x03)
    {
        byte[] dLen = new byte[1];
        await ReadExactStreamAsync(stream, dLen, 0, 1, ct);
        remBytes = dLen[0] + 2;
    }
    else
    {
        remBytes = 6;
    }

    byte[] discard = new byte[remBytes];
    await ReadExactStreamAsync(stream, discard, 0, remBytes, ct);
}
