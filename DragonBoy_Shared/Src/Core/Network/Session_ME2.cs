using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// CLASS Session_ME2: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Session_ME2 : ISession
{
	/// <summary>
	/// CLASS Sender: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class Sender
	{
		public List<Message> sendingMessage;

		/// <summary>
		/// Đóng gói và gửi thông điệp mạng Sender lên máy chủ.
		/// </summary>
		public Sender()
		{
			sendingMessage = new List<Message>();
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ AddMessage.
		/// </summary>
		public void AddMessage(Message message)
		{
			lock (sendingMessage)
			{
				sendingMessage.Add(message);
			}
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ run.
		/// </summary>
		public void run()
		{
			while (connected)
			{
				try
				{
					if (getKeyComplete)
					{
						while (true)
						{
							Message m = null;
							lock (sendingMessage)
							{
								if (sendingMessage.Count > 0)
								{
									m = sendingMessage[0];
									sendingMessage.RemoveAt(0);
								}
							}
							if (m == null)
							{
								break;
							}
							doSendMessage(m);
						}
					}
					try
					{
						int count = 0;
						lock (sendingMessage)
						{
							count = sendingMessage.Count;
						}
						if (count == 0)
						{
							Thread.Sleep(1);
						}
					}
					catch (Exception ex)
					{
						Cout.LogError(ex.ToString());
					}
				}
				catch (Exception)
				{
					Res.outz("error send message! ");
				}
			}
		}
	}

	/// <summary>
	/// CLASS MessageCollector: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	private class MessageCollector
	{
		/// <summary>
		/// Thực thi phương thức nghiệp vụ run.
		/// </summary>
		public void run()
		{
			try
			{
				while (connected)
				{
					Message message = readMessage();
					if (message == null)
					{
						break;
					}
					try
					{
						if (message.command == -27)
						{
							getKey(message);
						}
						else
						{
							onRecieveMsg(message);
						}
					}
					catch (Exception)
					{
						Cout.println("LOI NHAN  MESS THU 1");
					}
					try
					{
						if (dataStream != null && !dataStream.DataAvailable)
						{
							Thread.Sleep(1);
						}
					}
					catch (Exception)
					{
						Cout.println("LOI NHAN  MESS THU 2");
					}
				}
			}
			catch (Exception ex3)
			{
				Debug.Log("error read message!");
				Debug.Log(ex3.Message.ToString());
			}
			if (!connected)
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
			if (sc != null)
			{
				cleanNetwork();
			}
		}

		/// <summary>
		/// Truy xuất và trả về dữ liệu của getKey.
		/// </summary>
		private void getKey(Message message)
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
				getKeyComplete = true;
				GameMidlet.IP2 = message.reader().readUTF();
				GameMidlet.PORT2 = message.reader().readInt();
				GameMidlet.isConnect2 = ((message.reader().readByte() != 0) ? true : false);
				if (isMainSession && GameMidlet.isConnect2)
				{
					GameCanvas.connect2();
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ readMessage2.
		/// </summary>
		private Message readMessage2(sbyte cmd)
		{
			int num = readKey(dis.ReadSByte()) + 128;
			int num2 = readKey(dis.ReadSByte()) + 128;
			int num3 = readKey(dis.ReadSByte()) + 128;
			int num4 = (num3 * 256 + num2) * 256 + num;
			Cout.LogError("SIZE = " + num4);
			sbyte[] array = new sbyte[num4];
			int num5 = 0;
			byte[] src = dis.ReadBytes(num4);
			Buffer.BlockCopy(src, 0, array, 0, num4);
			recvByteCount += 5 + num4;
			int num6 = recvByteCount + sendByteCount;
			strRecvByteCount = num6 / 1024 + "." + num6 % 1024 / 102 + "Kb";
			if (getKeyComplete)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = readKey(array[i]);
				}
			}
			return new Message(cmd, array);
		}

		/// <summary>
		/// Thực thi phương thức nghiệp vụ readMessage.
		/// </summary>
		private Message readMessage()
		{
			try
			{
				sbyte b = dis.ReadSByte();
				if (getKeyComplete)
				{
					b = readKey(b);
				}
				if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87)
				{
					return readMessage2(b);
				}
				int num;
				if (getKeyComplete)
				{
					sbyte b2 = dis.ReadSByte();
					sbyte b3 = dis.ReadSByte();
					num = ((readKey(b2) & 0xFF) << 8) | (readKey(b3) & 0xFF);
				}
				else
				{
					sbyte b4 = dis.ReadSByte();
					sbyte b5 = dis.ReadSByte();
					num = ((b4 & 0xFF) << 8) | (b5 & 0xFF);
				}
				sbyte[] array = new sbyte[num];
				int num2 = 0;
				int num3 = 0;
				byte[] src = dis.ReadBytes(num);
				Buffer.BlockCopy(src, 0, array, 0, num);
				recvByteCount += 5 + num;
				int num4 = recvByteCount + sendByteCount;
				strRecvByteCount = num4 / 1024 + "." + num4 % 1024 / 102 + "Kb";
				if (getKeyComplete)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = readKey(array[i]);
					}
				}
				return new Message(b, array);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.StackTrace.ToString());
			}
			return null;
		}
	}

	protected static Session_ME2 instance = new Session_ME2();

	private static NetworkStream dataStream;

	private static BinaryReader dis;

	private static BinaryWriter dos;

	public static IMessageHandler messageHandler;

	public static bool isMainSession = true;

	private static TcpClient sc;

	public static bool connected;

	public static bool connecting;

	private static Sender sender = new Sender();

	public static Thread initThread;

	public static Thread collectorThread;

	public static Thread sendThread;

	public static int sendByteCount;

	public static int recvByteCount;

	private static bool getKeyComplete;

	public static sbyte[] key = null;

	private static sbyte curR;

	private static sbyte curW;

	private static int timeConnected;

	private long lastTimeConn;

	public static string strRecvByteCount = string.Empty;

	public static bool isCancel;

	private string host;

	private int port;

	private long timeWaitConnect;

	public static MyVector recieveMsg = new MyVector();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Session_ME2.
	/// </summary>
	public Session_ME2()
	{
		Debug.Log("init Session_ME");
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearSendingMessage.
	/// </summary>
	public void clearSendingMessage()
	{
		try
		{
			lock (sender.sendingMessage)
			{
				sender.sendingMessage.Clear();
			}
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ gI.
	/// </summary>
	public static Session_ME2 gI()
	{
		if (instance == null)
		{
			instance = new Session_ME2();
		}
		return instance;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isConnected.
	/// </summary>
	public bool isConnected()
	{
#if BROWSER
		if (OperatingSystem.IsBrowser())
		{
			return connected && _ws != null && _ws.State == System.Net.WebSockets.WebSocketState.Open;
		}
#endif
		return connected && sc != null && dis != null;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setHandler.
	/// </summary>
	public void setHandler(IMessageHandler msgHandler)
	{
		messageHandler = msgHandler;
	}

	/// <summary>
	/// Khởi tạo và thiết lập kết nối socket TCP tới máy chủ trò chơi.
	/// </summary>
	public void connect(string host, int port)
	{
		if (!connected && !connecting && mSystem.currentTimeMillis() >= timeWaitConnect)
		{
			timeWaitConnect = mSystem.currentTimeMillis() + 50;
			this.host = host;
			this.port = port;
			getKeyComplete = false;
			close();
			Debug.Log("connecting...!");
			Debug.Log("host: " + host);
			Debug.Log("port: " + port);
#if BROWSER
			if (OperatingSystem.IsBrowser())
			{
				_ = ConnectWebSocketAsync(host, port);
				return;
			}
#endif
			initThread = new Thread(NetworkInit);
			initThread.Start();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ NetworkInit.
	/// </summary>
	private void NetworkInit()
	{
		isCancel = false;
		connecting = true;
		Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
		connected = true;
		try
		{
			doConnect(host, port);
			messageHandler.onConnectOK(isMainSession);
		}
		catch (Exception)
		{
			if (messageHandler != null)
			{
				close();
				messageHandler.onConnectionFail(isMainSession);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ doConnect.
	/// </summary>
	public void doConnect(string host, int port)
	{
		Console.WriteLine($"[NET] Connecting to {host}:{port}...");
		sc = ModProxyServer.ConnectTcpClient(host, port);
		Console.WriteLine($"[NET] sc.Connect completed! Setting up streams...");
		dataStream = sc.GetStream();
		dis = new BinaryReader(dataStream, new UTF8Encoding());
		dos = new BinaryWriter(dataStream, new UTF8Encoding());
		sendThread = new Thread(sender.run);
		sendThread.Start();
		MessageCollector @object = new MessageCollector();
		Cout.LogError("new -----");
		collectorThread = new Thread(@object.run);
		collectorThread.Start();
		timeConnected = currentTimeMillis();
		connecting = false;
		doSendMessage(new Message(-27));
	}

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendMessage lên máy chủ.
	/// </summary>
	public void sendMessage(Message message)
	{
		Res.outz("SEND MSG: " + message.command);
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
			lock (recieveMsg)
			{
				recieveMsg.addElement(msg);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
		while (true)
		{
			if (Controller.isStopReadMessage)
			{
				break;
			}
			Message message = null;
			lock (recieveMsg)
			{
				if (recieveMsg.size() > 0)
				{
					message = (Message)recieveMsg.elementAt(0);
					recieveMsg.removeElementAt(0);
				}
			}
			if (message == null)
			{
				break;
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
		curR = 0;
		curW = 0;
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
			sendThread = null;
			collectorThread = null;
			instance.timeWaitConnect = 0;
		}
		catch (Exception)
		{
		}
	}

#if BROWSER
	private static System.Net.WebSockets.ClientWebSocket _ws;
	private static CancellationTokenSource _wsCts;

	private sealed class WsStreamReader2
	{
		private readonly System.Net.WebSockets.ClientWebSocket _socket;
		private readonly byte[] _recvBuf = new byte[65536];
		private int _pos;
		private int _len;

		public WsStreamReader2(System.Net.WebSockets.ClientWebSocket socket)
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
					throw new EndOfStreamException("WebSocket2 closed by remote host.");
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
						throw new EndOfStreamException($"WebSocket2 closed before reading {size} bytes.");
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
		catch (Exception)
		{
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
				while (true)
				{
					Message m = null;
					lock (sender.sendingMessage)
					{
						if (sender.sendingMessage.Count > 0)
						{
							m = sender.sendingMessage[0];
							sender.sendingMessage.RemoveAt(0);
						}
					}
					if (m == null) break;
					await WsSendRawMessageAsync(ws, m, ct);
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
			GameMidlet.IP2 = message.reader().readUTF();
			GameMidlet.PORT2 = message.reader().readInt();
			GameMidlet.isConnect2 = message.reader().readByte() != 0;
			if (isMainSession && GameMidlet.isConnect2)
			{
				GameCanvas.connect2();
			}
		}
		catch (Exception)
		{
		}
	}

	private static async System.Threading.Tasks.Task<Message> WsReadMessageAsync(WsStreamReader2 reader, CancellationToken ct)
	{
		sbyte b = await reader.ReadSByteAsync(ct);
		if (getKeyComplete)
		{
			b = readKey(b);
		}
		if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87)
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
		var reader = new WsStreamReader2(ws);
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
					onRecieveMsg(message);
				}
			}
		}
		catch (Exception)
		{
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
	/// Thực thi phương thức nghiệp vụ currentTimeMillis.
	/// </summary>
	public static int currentTimeMillis()
	{
		return Environment.TickCount;
	}

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
}
