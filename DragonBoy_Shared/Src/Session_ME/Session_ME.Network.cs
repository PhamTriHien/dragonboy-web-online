using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Session_ME: Đảm nhiệm phân hệ chuyên trách Network ().
/// </summary>
public partial class Session_ME
{
	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendMessage lên máy chủ.
	/// </summary>
	public void sendMessage(Message message)
		{
			count++;
			Res.outz("SEND MSG: " + message.command);
			lastSendTime = mSystem.currentTimeMillis();
			sender.AddMessage(message);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ doSendMessage.
	/// </summary>
	private static void doSendMessage(Message m)
	{
		sbyte[] data = m.getData();
		try
		{
			if (getKeyComplete)
			{
				sbyte value = writeKey(m.command);
				dos.Write(value);
			}
			else
			{
				dos.Write(m.command);
			}
			if (data != null)
			{
				int num = data.Length;
				if (getKeyComplete)
				{
					int num2 = writeKey((sbyte)(num >> 8));
					dos.Write((sbyte)num2);
					int num3 = writeKey((sbyte)(num & 0xFF));
					dos.Write((sbyte)num3);
				}
				else
				{
					dos.Write((sbyte)(num >> 8));
					dos.Write((sbyte)(num & 0xFF));
				}
				if (getKeyComplete)
				{
					for (int i = 0; i < data.Length; i++)
					{
						sbyte value2 = writeKey(data[i]);
						dos.Write(value2);
					}
				}
				else
				{
					for (int i = 0; i < data.Length; i++)
					{
						dos.Write(data[i]);
					}
				}
				sendByteCount += 5 + data.Length;
			}
			else
			{
				if (getKeyComplete)
				{
					int num4 = 0;
					int num5 = writeKey((sbyte)(num4 >> 8));
					dos.Write((sbyte)num5);
					int num6 = writeKey((sbyte)(num4 & 0xFF));
					dos.Write((sbyte)num6);
				}
				else
				{
					dos.Write((sbyte)0);
					dos.Write((sbyte)0);
				}
				sendByteCount += 5;
			}
			dos.Flush();
		}
		catch (Exception ex)
		{
			Debug.Log(ex.StackTrace);
			try { dos.Flush(); } catch { }
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ readKey.
	/// </summary>
	public static sbyte readKey(sbyte b)
		{
			sbyte[] array = key;
			sbyte num = curR;
			curR = (sbyte)(num + 1);
			sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
			if (curR >= key.Length)
			{
				curR %= (sbyte)key.Length;
			}
			return result;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ writeKey.
	/// </summary>
	public static sbyte writeKey(sbyte b)
		{
			sbyte[] array = key;
			sbyte num = curW;
			curW = (sbyte)(num + 1);
			sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
			if (curW >= key.Length)
			{
				curW %= (sbyte)key.Length;
			}
			return result;
		}

	public static readonly System.Collections.Concurrent.ConcurrentQueue<Message> msgQueue = new System.Collections.Concurrent.ConcurrentQueue<Message>();

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi onRecieveMsg.
	/// </summary>
	public static void onRecieveMsg(Message msg)
		{
			if (Thread.CurrentThread.Name == Main.mainThreadName)
			{
				messageHandler.onMessage(msg);
			}
			else
			{
				msgQueue.Enqueue(msg);
			}
		}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
		{
			if (recieveMsg.size() > 0)
			{
				lock (recieveMsg)
				{
					while (recieveMsg.size() > 0)
					{
						msgQueue.Enqueue((Message)recieveMsg.elementAt(0));
						recieveMsg.removeElementAt(0);
					}
				}
			}

			while (!Controller.isStopReadMessage && msgQueue.TryDequeue(out Message message))
			{
				if (lastSendTime > 0)
				{
					long rtt = mSystem.currentTimeMillis() - lastSendTime;
					if (rtt >= 1 && rtt <= 800)
					{
						ModMenu.pingMs = (int)rtt;
					}
					lastSendTime = 0;
				}
				messageHandler.onMessage(message);
			}
		}

	/// <summary>
	/// Đóng cửa sổ hoặc giải phóng phiên làm việc hiện tại.
	/// </summary>
	public void close()
		{
			cleanNetwork();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ cleanNetwork.
	/// </summary>
	private static void cleanNetwork()
		{
			key = null;
			extendedKey = null;
			curR = 0;
			curW = 0;
			getKeyComplete = false;
			while (msgQueue.TryDequeue(out _)) { }
			if (sender != null)
			{
				while (sender.sendingQueue.TryDequeue(out _)) { }
				lock (sender.sendingMessage)
				{
					sender.sendingMessage.Clear();
				}
			}
			Debug.LogError(">>>cleanNetwork ...!");
			try
			{
				connected = false;
				connecting = false;
#if BROWSER
				if (_wsCts != null)
				{
					try { _wsCts.Cancel(); _wsCts.Dispose(); } catch { }
					_wsCts = null;
				}
				if (_ws != null)
				{
					try { _ws.Dispose(); } catch { }
					_ws = null;
				}
#endif
				if (sc != null)
				{
					sc.Close();
					sc = null;
				}
				if (dataStream != null)
				{
					dataStream.Close();
					dataStream = null;
				}
				if (dos != null)
				{
					dos.Close();
					dos = null;
				}
				if (dis != null)
				{
					dis.Close();
					dis = null;
				}
				if (Thread.CurrentThread.Name == Main.mainThreadName)
				{
					if (sendThread != null)
					{
						try { sendThread.Interrupt(); } catch { }
					}
					sendThread = null;
					if (initThread != null)
					{
						try { initThread.Interrupt(); } catch { }
					}
					initThread = null;
					if (collectorThread != null)
					{
						try { collectorThread.Interrupt(); } catch { }
					}
					collectorThread = null;
				}
				else
				{
					sendThread = null;
					initThread = null;
					collectorThread = null;
				}
				if (isMainSession)
				{
					ServerListScreen.testConnect = 0;
				}
				if (instance != null)
				{
					instance.timeWaitConnect = 0;
				}
				Controller.isGet_CLIENT_INFO = false;
			}
			catch (Exception)
			{
			}
		}

#if BROWSER
	private static System.Net.WebSockets.ClientWebSocket _ws;
	private static CancellationTokenSource _wsCts;

	private sealed class WsStreamReader
	{
		private readonly System.Net.WebSockets.ClientWebSocket _socket;
		private readonly byte[] _recvBuf = new byte[65536];
		private int _pos;
		private int _len;

		public WsStreamReader(System.Net.WebSockets.ClientWebSocket socket)
		{
			_socket = socket;
		}

		public async System.Threading.Tasks.Task<sbyte> ReadSByteAsync(CancellationToken ct)
		{
			while (_pos >= _len)
			{
				var res = await _socket.ReceiveAsync(new ArraySegment<byte>(_recvBuf), ct);
				if (res.MessageType == System.Net.WebSockets.WebSocketMessageType.Close || res.Count <= 0)
				{
					throw new EndOfStreamException("WebSocket closed by remote host.");
				}
				_pos = 0;
				_len = res.Count;
			}
			return (sbyte)_recvBuf[_pos++];
		}

		public async System.Threading.Tasks.Task<byte[]> ReadBytesAsync(int size, CancellationToken ct)
		{
			byte[] dst = new byte[size];
			int offset = 0;
			while (offset < size)
			{
				if (_pos >= _len)
				{
					var res = await _socket.ReceiveAsync(new ArraySegment<byte>(_recvBuf), ct);
					if (res.MessageType == System.Net.WebSockets.WebSocketMessageType.Close || res.Count <= 0)
					{
						throw new EndOfStreamException($"WebSocket closed before reading {size} bytes (read {offset}).");
					}
					_pos = 0;
					_len = res.Count;
				}
				int take = System.Math.Min(size - offset, _len - _pos);
				Buffer.BlockCopy(_recvBuf, _pos, dst, offset, take);
				_pos += take;
				offset += take;
			}
			return dst;
		}
	}

	private async System.Threading.Tasks.Task ConnectWebSocketAsync(string targetHost, int targetPort)
	{
		isCancel = false;
		connecting = true;
		connected = true;
		var cts = new CancellationTokenSource();
		var ws = new System.Net.WebSockets.ClientWebSocket();
		_wsCts = cts;
		_ws = ws;
		try
		{
			Uri relayUri = DragonBoy_Web_Host.WebNetworkConfig.BuildRelayUri(targetHost, targetPort);
			await ws.ConnectAsync(relayUri, cts.Token);
			if (_ws != ws || cts.IsCancellationRequested)
			{
				return;
			}
			timeConnected = currentTimeMillis();
			connecting = false;
			await WsSendRawMessageAsync(ws, new Message(-27), cts.Token);
			_ = WsSendLoopAsync(ws, cts.Token);
			_ = WsReceiveLoopAsync(ws, cts.Token);
			messageHandler?.onConnectOK(isMainSession);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[WS NET EX] {ex.GetType().Name}: {ex.Message}");
			await System.Threading.Tasks.Task.Delay(100);
			if (_ws == ws && !cts.IsCancellationRequested && !isCancel && messageHandler != null)
			{
				close();
				messageHandler.onConnectionFail(isMainSession);
			}
		}
	}

	private static byte[] SerializeMessagePacket(Message m)
	{
		sbyte[] data = m.getData();
		int len = data != null ? data.Length : 0;
		byte[] packet = new byte[3 + len];
		packet[0] = (byte)(getKeyComplete ? writeKey(m.command) : m.command);
		if (getKeyComplete)
		{
			packet[1] = (byte)writeKey((sbyte)(len >> 8));
			packet[2] = (byte)writeKey((sbyte)(len & 0xFF));
			for (int i = 0; i < len; i++)
			{
				packet[3 + i] = (byte)writeKey(data[i]);
			}
		}
		else
		{
			packet[1] = (byte)(len >> 8);
			packet[2] = (byte)(len & 0xFF);
			for (int i = 0; i < len; i++)
			{
				packet[3 + i] = (byte)data[i];
			}
		}
		sendByteCount += 5 + len;
		return packet;
	}

	private static async System.Threading.Tasks.Task WsSendRawMessageAsync(System.Net.WebSockets.ClientWebSocket ws, Message m, CancellationToken ct)
	{
		if (ws == null || ws.State != System.Net.WebSockets.WebSocketState.Open) return;
		byte[] packet = SerializeMessagePacket(m);
		await ws.SendAsync(new ArraySegment<byte>(packet), System.Net.WebSockets.WebSocketMessageType.Binary, true, ct);
	}

	private static async System.Threading.Tasks.Task WsSendLoopAsync(System.Net.WebSockets.ClientWebSocket ws, CancellationToken ct)
	{
		try
		{
			while (connected && !ct.IsCancellationRequested && ws.State == System.Net.WebSockets.WebSocketState.Open)
			{
				if (!getKeyComplete)
				{
					await System.Threading.Tasks.Task.Delay(5, ct);
					continue;
				}
				if (sender.sendingMessage.Count > 0)
				{
					lock (sender.sendingMessage)
					{
						while (sender.sendingMessage.Count > 0)
						{
							sender.sendingQueue.Enqueue(sender.sendingMessage[0]);
							sender.sendingMessage.RemoveAt(0);
						}
					}
				}
				while (sender.sendingQueue.TryDequeue(out Message m))
				{
					if (m != null)
					{
						await WsSendRawMessageAsync(ws, m, ct);
					}
				}
				await System.Threading.Tasks.Task.Delay(5, ct);
			}
		}
		catch (Exception)
		{
		}
	}

	private static void WsHandleGetKey(Message message)
	{
		try
		{
			sbyte b = message.reader().readSByte();
			key = new sbyte[b];
			for (int i = 0; i < b; i++)
			{
				key[i] = message.reader().readSByte();
			}
			for (int j = 0; j < key.Length - 1; j++)
			{
				ref sbyte reference = ref key[j + 1];
				reference ^= key[j];
			}
			curR = 0;
			curW = 0;
			getKeyComplete = true;
			try
			{
				GameMidlet.IP2 = message.reader().readUTF();
				GameMidlet.PORT2 = message.reader().readInt();
				GameMidlet.isConnect2 = message.reader().readByte() != 0;
				if (isMainSession && GameMidlet.isConnect2)
				{
					GameCanvas.connect2();
				}
			}
			catch { }
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[WS GETKEY EX] {ex}");
		}
	}

	private static async System.Threading.Tasks.Task<Message> WsReadMessageAsync(WsStreamReader reader, CancellationToken ct)
	{
		sbyte b = await reader.ReadSByteAsync(ct);
		if (getKeyComplete)
		{
			b = readKey(b);
		}
		if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87 || b == 66 || b == 12)
		{
			int num = readKey(await reader.ReadSByteAsync(ct)) + 128;
			int num2 = readKey(await reader.ReadSByteAsync(ct)) + 128;
			int num3 = readKey(await reader.ReadSByteAsync(ct)) + 128;
			int num4 = (num3 * 256 + num2) * 256 + num;
			byte[] src24 = await reader.ReadBytesAsync(num4, ct);
			sbyte[] array24 = new sbyte[num4];
			Buffer.BlockCopy(src24, 0, array24, 0, num4);
			recvByteCount += 5 + num4;
			int total24 = recvByteCount + sendByteCount;
			strRecvByteCount = total24 / 1024 + "." + total24 % 1024 / 102 + "Kb";
			if (getKeyComplete)
			{
				for (int i = 0; i < array24.Length; i++)
				{
					array24[i] = readKey(array24[i]);
				}
			}
			return new Message(b, array24);
		}
		int size;
		if (getKeyComplete)
		{
			sbyte b2 = await reader.ReadSByteAsync(ct);
			sbyte b3 = await reader.ReadSByteAsync(ct);
			size = ((readKey(b2) & 0xFF) << 8) | (readKey(b3) & 0xFF);
		}
		else
		{
			sbyte b4 = await reader.ReadSByteAsync(ct);
			sbyte b5 = await reader.ReadSByteAsync(ct);
			size = ((b4 & 0xFF) << 8) | (b5 & 0xFF);
		}
		byte[] src = await reader.ReadBytesAsync(size, ct);
		sbyte[] array = new sbyte[size];
		Buffer.BlockCopy(src, 0, array, 0, size);
		recvByteCount += 5 + size;
		int total = recvByteCount + sendByteCount;
		strRecvByteCount = total / 1024 + "." + total % 1024 / 102 + "Kb";
		if (getKeyComplete)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = readKey(array[i]);
			}
		}
		return new Message(b, array);
	}

	private static async System.Threading.Tasks.Task WsReceiveLoopAsync(System.Net.WebSockets.ClientWebSocket ws, CancellationToken ct)
	{
		var reader = new WsStreamReader(ws);
		try
		{
			while (connected && !ct.IsCancellationRequested && _ws == ws && ws.State == System.Net.WebSockets.WebSocketState.Open)
			{
				Message message = await WsReadMessageAsync(reader, ct);
				if (message == null) break;
				if (message.command == -27)
				{
					WsHandleGetKey(message);
				}
				else
				{
					msgQueue.Enqueue(message);
				}
			}
		}
		catch (Exception ex)
		{
			if (!ct.IsCancellationRequested)
			{
				Console.WriteLine($"[WS COLLECTOR EX] {ex.Message}");
			}
		}
		if (!connected || ct.IsCancellationRequested || _ws != ws)
		{
			return;
		}
		if (messageHandler != null)
		{
			if (currentTimeMillis() - timeConnected > 500)
			{
				messageHandler.onDisconnected(isMainSession);
			}
			else
			{
				messageHandler.onConnectionFail(isMainSession);
			}
		}
		cleanNetwork();
	}
#endif

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertSbyteToByte.
	/// </summary>
	public static byte convertSbyteToByte(sbyte var)
		{
			if (var > 0)
			{
				return (byte)var;
			}
			return (byte)(var + 256);
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ convertSbyteToByte.
	/// </summary>
	public static byte[] convertSbyteToByte(sbyte[] var)
		{
			byte[] array = new byte[var.Length];
			for (int i = 0; i < var.Length; i++)
			{
				if (var[i] > 0)
				{
					array[i] = (byte)var[i];
				}
				else
				{
					array[i] = (byte)(var[i] + 256);
				}
			}
			return array;
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isCompareIPConnect.
	/// </summary>
	public bool isCompareIPConnect()
		{
			return true;
		}

}
