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
/// Phiên kết nối mạng Socket TCP nhị phân: Quản lý luồng gửi/nhận, bắt tay khóa bảo mật và giải mã gói tin.
/// </summary>
public partial class Session_ME : ISession
{
	/// <summary>
	/// CLASS Sender: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class Sender
		{
			public readonly System.Collections.Concurrent.ConcurrentQueue<Message> sendingQueue = new System.Collections.Concurrent.ConcurrentQueue<Message>();
			public readonly AutoResetEvent sendEvent = new AutoResetEvent(false);
			public List<Message> sendingMessage = new List<Message>();

			/// <summary>
			/// Đóng gói và gửi thông điệp mạng Sender lên máy chủ.
			/// </summary>
			public Sender()
			{
			}

			/// <summary>
			/// Thực thi phương thức nghiệp vụ AddMessage.
			/// </summary>
			public void AddMessage(Message message)
			{
				sendingQueue.Enqueue(message);
				sendEvent.Set();
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
						if (!getKeyComplete)
						{
							Thread.Sleep(5);
							continue;
						}

						sendEvent.WaitOne(100);
						if (!connected) break;

						if (sendingMessage.Count > 0)
						{
							lock (sendingMessage)
							{
								while (sendingMessage.Count > 0)
								{
									sendingQueue.Enqueue(sendingMessage[0]);
									sendingMessage.RemoveAt(0);
								}
							}
						}

						while (sendingQueue.TryDequeue(out Message m))
						{
							if (m != null)
							{
								doSendMessage(m);
							}
						}
					}
					catch (Exception ex)
					{
						Cout.LogError(ex.ToString());
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
							Console.WriteLine("[NET COLLECTOR] readMessage returned null, breaking collector loop! connected=" + connected);
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
						catch (Exception ex1)
						{
							Cout.println("LOI NHAN  MESS THU 1: " + ex1);
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
					Console.WriteLine("[NET COLLECTOR EX] " + ex3.ToString());
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
						Console.WriteLine("[NET COLLECTOR] Triggering onDisconnected");
						messageHandler.onDisconnected(isMainSession);
					}
					else
					{
						Console.WriteLine("[NET COLLECTOR] Triggering onConnectionFail");
						messageHandler.onConnectionFail(isMainSession);
					}
				}
				if (sc != null)
				{
					cleanNetwork();
				}
			}

			/// <summary>
			/// Thực thi phương thức nghiệp vụ ReadFully.
			/// </summary>
			private static byte[] ReadFully(BinaryReader reader, int size)
			{
				byte[] buffer = new byte[size];
				int totalBytesRead = 0;
				while (totalBytesRead < size)
				{
					int bytesRead = reader.Read(buffer, totalBytesRead, size - totalBytesRead);
					if (bytesRead <= 0)
					{
						throw new EndOfStreamException($"Socket closed before reading {size} bytes (read {totalBytesRead} bytes).");
					}
					totalBytesRead += bytesRead;
				}
				return buffer;
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
					curR = 0;
					curW = 0;
					getKeyComplete = true;
					sender.sendEvent.Set();
					Console.WriteLine($"[NET] Key exchange completed! Key length: {b}");
					try
					{
						GameMidlet.IP2 = message.reader().readUTF();
						GameMidlet.PORT2 = message.reader().readInt();
						GameMidlet.isConnect2 = ((message.reader().readByte() != 0) ? true : false);
						if (isMainSession && GameMidlet.isConnect2)
						{
							GameCanvas.connect2();
						}
					}
					catch { }
				}
				catch (Exception ex)
				{
					Console.WriteLine($"[NET GETKEY EX] {ex}");
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
				sbyte[] array = new sbyte[num4];
				byte[] src = ReadFully(dis, num4);
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
					if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87 || b == 66 || b == 12)
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
					byte[] src = ReadFully(dis, num);
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
					Console.WriteLine($"[NET READ EX] {ex.GetType().Name}: {ex.Message}");
					Debug.Log(ex.StackTrace.ToString());
				}
				return null;
			}
		}

	protected static Session_ME instance = new Session_ME();

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

	public static bool getKeyComplete;

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isKeyComplete.
	/// </summary>
	public static bool isKeyComplete()
	{
		return getKeyComplete;
	}

	public static sbyte[] key = null;

	public static sbyte[] extendedKey = null;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DecryptPayload.
	/// </summary>
	public static void DecryptPayload(sbyte[] data)
	{
		if (data == null || data.Length == 0 || !getKeyComplete || key == null || key.Length == 0)
		{
			return;
		}

		int len = data.Length;
		int kLen = key.Length;
		int r = curR;
		int i = 0;

		// SIMD AVX2 Hardware Vectorization: 32 bytes per cycle
		if (System.Runtime.Intrinsics.Vector256.IsHardwareAccelerated && len >= 32 && extendedKey != null)
		{
			while (i + 32 <= len)
			{
				var vData = System.Runtime.Intrinsics.Vector256.LoadUnsafe(ref data[i]);
				var vKey = System.Runtime.Intrinsics.Vector256.LoadUnsafe(ref extendedKey[r]);
				var vRes = vData ^ vKey;
				System.Runtime.Intrinsics.Vector256.StoreUnsafe(vRes, ref data[i]);
				i += 32;
				r = (r + 32) % kLen;
			}
		}

		// SIMD SSE2 / Vector128 Vectorization: 16 bytes per cycle
		if (System.Runtime.Intrinsics.Vector128.IsHardwareAccelerated && (len - i) >= 16 && extendedKey != null)
		{
			while (i + 16 <= len)
			{
				var vData = System.Runtime.Intrinsics.Vector128.LoadUnsafe(ref data[i]);
				var vKey = System.Runtime.Intrinsics.Vector128.LoadUnsafe(ref extendedKey[r]);
				var vRes = vData ^ vKey;
				System.Runtime.Intrinsics.Vector128.StoreUnsafe(vRes, ref data[i]);
				i += 16;
				r = (r + 16) % kLen;
			}
		}

		// High-performance CPU register scalar tail
		while (i < len)
		{
			data[i] ^= key[r];
			r++;
			if (r >= kLen) r = 0;
			i++;
		}

		curR = (sbyte)r;
	}

	private static sbyte curR;

	private static sbyte curW;

	private static int timeConnected;

	private long lastTimeConn;

	public static string strRecvByteCount = string.Empty;

	public static long lastSendTime = 0;

	public static bool isCancel;

	private string host;

	private int port;

	private long timeWaitConnect;

	public static int count;

	public static MyVector recieveMsg = new MyVector();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Session_ME.
	/// </summary>
	public Session_ME()
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
				while (sender.sendingQueue.TryDequeue(out _)) { }
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
	public static Session_ME gI()
		{
			if (instance == null)
			{
				instance = new Session_ME();
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
		if (connected || connecting)
		{
			if (this.host == host && this.port == port)
			{
				return;
			}
			close();
		}
		timeWaitConnect = mSystem.currentTimeMillis() + 300;
		if (isMainSession)
		{
			ServerListScreen.testConnect = -1;
		}
		this.host = host;
		this.port = port;
		getKeyComplete = false;
		close();
		connecting = true;
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ NetworkInit.
	/// </summary>
	private void NetworkInit()
		{
			isCancel = false;
			connecting = true;
			connected = true;
			Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
			try
			{
				Console.WriteLine($"[NET] doConnect starting for {host}:{port}...");
				doConnect(host, port);
				Console.WriteLine($"[NET] doConnect succeeded! Calling onConnectOK...");
				messageHandler.onConnectOK(isMainSession);
			}
			catch (Exception ex)
			{
				connected = false;
				connecting = false;
				Console.WriteLine($"[NET CONNECT EX] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
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
		IPAddress ipAddr;
			if (!IPAddress.TryParse(host, out ipAddr))
			{
				IPAddress[] ips = Dns.GetHostAddresses(host);
				ipAddr = null;
				for (int i = 0; i < ips.Length; i++)
				{
					if (ips[i].AddressFamily == AddressFamily.InterNetwork)
					{
						ipAddr = ips[i];
						break;
					}
				}
				if (ipAddr == null && ips.Length > 0)
				{
					ipAddr = ips[0];
				}
			}
			sc = new TcpClient(ipAddr.AddressFamily);
			sc.NoDelay = true;
			sc.SendBufferSize = 65536;
			sc.ReceiveBufferSize = 131072;
			try
			{
				sc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
				sc.Client.LingerState = new LingerOption(false, 0);
			}
			catch { }
			Console.WriteLine($"[NET] Connecting to single IP {ipAddr}:{port}...");
			sc.Connect(ipAddr, port);
			Console.WriteLine($"[NET] sc.Connect completed! Setting up streams...");
			dataStream = sc.GetStream();
			dis = new BinaryReader(dataStream, new UTF8Encoding());
			dos = new BinaryWriter(dataStream, new UTF8Encoding());
			sendThread = new Thread(sender.run);
			sendThread.Name = "NRO_SendThread";
			sendThread.IsBackground = true;
			sendThread.Priority = ThreadPriority.AboveNormal;
			sendThread.Start();
			key = null;
			MessageCollector @object = new MessageCollector();
			collectorThread = new Thread(@object.run);
			collectorThread.Name = "NRO_CollectorThread";
			collectorThread.IsBackground = true;
			collectorThread.Priority = ThreadPriority.Highest;
			collectorThread.Start();
			timeConnected = currentTimeMillis();
			connecting = false;
			doSendMessage(new Message(-27));
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ currentTimeMillis.
	/// </summary>
	public static int currentTimeMillis()
		{
			return Environment.TickCount;
		}

}
