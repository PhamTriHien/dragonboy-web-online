using System.Threading;
using UnityEngine;

/// <summary>
/// CLASS Sound: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Sound
{
	private const int INTERVAL = 5;

	private const int MAXTIME = 100;

	public static int status;

	public static int postem;

	public static int timestart;

	private static string filenametemp;

	private static float volumetem;

	public static bool isSound = true;

	public static bool isNotPlay;

	public static bool stopAll;

	public static AudioSource SoundWater;

	public static AudioSource SoundRun;

	public static AudioSource SoundBGLoop;

	public static AudioClip[] music;

	public static GameObject[] player;

	public static sbyte MLogin;

	public static sbyte MBClick = 1;

	public static sbyte MTone = 2;

	public static sbyte MSanzu = 3;

	public static sbyte MChakumi = 4;

	public static sbyte MChai = 5;

	public static sbyte MOshin = 6;

	public static sbyte MEchigo = 7;

	public static sbyte MKojin = 8;

	public static sbyte MHaruna = 9;

	public static sbyte MHirosaki = 10;

	public static sbyte MOokaza = 11;

	public static sbyte MGiotuyet = 12;

	public static sbyte MHangdong = 13;

	public static sbyte MDeKeu = 14;

	public static sbyte MChimKeu = 15;

	public static sbyte MBuocChan = 16;

	public static sbyte MNuocChay = 17;

	public static sbyte MBomMau = 18;

	public static sbyte MKiemGo = 19;

	public static sbyte MKiem = 20;

	public static sbyte MTieu = 21;

	public static sbyte MKunai = 22;

	public static sbyte MCung = 23;

	public static sbyte MDao = 24;

	public static sbyte MQuat = 25;

	public static sbyte MCung2 = 26;

	public static sbyte MTieu2 = 27;

	public static sbyte MTieu3 = 28;

	public static sbyte MKiem2 = 29;

	public static sbyte MKiem3 = 30;

	public static sbyte MDao2 = 31;

	public static sbyte MDao3 = 32;

	public static sbyte MCung3 = 33;

	public static int l1;

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setActivity.
	/// </summary>
	public static void setActivity(SoundMn.AssetManager ac)
	{
	}

	/// <summary>
	/// Dừng tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public static void stop()
	{
		if (player == null) return;
		for (int i = 0; i < player.Length; i++)
		{
			if (player[i] != null)
			{
				player[i].GetComponent<AudioSource>()?.Pause();
			}
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlaying.
	/// </summary>
	public static bool isPlaying()
	{
		return false;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho init.
	/// </summary>
	public static void init()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "Audio Player";
		gameObject.transform.position = Vector3.zero;
		gameObject.AddComponent<AudioListener>();
		SoundBGLoop = gameObject.AddComponent<AudioSource>();
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho init.
	/// </summary>
	public static void init(int[] musicID, int[] sID)
	{
		if (player == null && music == null)
		{
			init();
			l1 = musicID.Length;
			player = new GameObject[musicID.Length + sID.Length];
			music = new AudioClip[musicID.Length + sID.Length];
			for (int i = 0; i < player.Length; i++)
			{
				string fileName = ((i >= l1) ? ("/sound/" + (i - l1)) : ("/music/" + i));
				getAssetSoundFile(fileName, i);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playSound.
	/// </summary>
	public static void playSound(int id, float volume)
	{
		play(id + l1, volume);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playSound1.
	/// </summary>
	public static void playSound1(int id, float volume)
	{
		play(id, volume);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getAssetSoundFile.
	/// </summary>
	public static void getAssetSoundFile(string fileName, int pos)
	{
		stop(pos);
		string empty = string.Empty;
		empty = Main.res + fileName;
		load(empty, pos);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopAllz.
	/// </summary>
	public static void stopAllz()
	{
		if (music != null)
		{
			for (int i = 0; i < music.Length; i++)
			{
				stop(i);
			}
		}
		for (int j = 0; j < l1; j++)
		{
			sTopSoundBG(j);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopAllBg.
	/// </summary>
	public static void stopAllBg()
	{
		for (int i = 0; i < music.Length; i++)
		{
			stop(i);
		}
		sTopSoundBG(0);
		sTopSoundRun();
		stopSoundNatural(0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void update()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopMusic.
	/// </summary>
	public static void stopMusic(int x)
	{
		if (GameCanvas.isPlaySound)
		{
			stop(x);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ play.
	/// </summary>
	public static void play(int id, float volume)
	{
		if (!isNotPlay && GameCanvas.isPlaySound)
		{
			start(volume, id);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playSoundRun.
	/// </summary>
	public static void playSoundRun(int id, float volume)
	{
		if (GameCanvas.isPlaySound && !(SoundRun == null))
		{
			SoundRun.GetComponent<AudioSource>().loop = true;
			SoundRun.GetComponent<AudioSource>().clip = music[id];
			SoundRun.GetComponent<AudioSource>().volume = volume;
			SoundRun.GetComponent<AudioSource>().Play();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ sTopSoundRun.
	/// </summary>
	public static void sTopSoundRun()
	{
		if (SoundRun != null)
		{
			SoundRun.GetComponent<AudioSource>().Stop();
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlayingSound.
	/// </summary>
	public static bool isPlayingSound()
	{
		if (SoundRun == null)
		{
			return false;
		}
		return SoundRun.GetComponent<AudioSource>().isPlaying;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playSoundNatural.
	/// </summary>
	public static void playSoundNatural(int id, float volume, bool isLoop)
	{
		if (GameCanvas.isPlaySound && !(SoundWater == null))
		{
			SoundWater.GetComponent<AudioSource>().loop = isLoop;
			SoundWater.GetComponent<AudioSource>().clip = music[id];
			SoundWater.GetComponent<AudioSource>().volume = volume;
			SoundWater.GetComponent<AudioSource>().Play();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ stopSoundNatural.
	/// </summary>
	public static void stopSoundNatural(int id)
	{
		if (SoundWater != null)
		{
			SoundWater.GetComponent<AudioSource>().Stop();
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlayingSoundatural.
	/// </summary>
	public static bool isPlayingSoundatural(int id)
	{
		if (SoundWater == null)
		{
			return false;
		}
		return SoundWater.GetComponent<AudioSource>().isPlaying;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playMus.
	/// </summary>
	public static void playMus(int type, float vl, bool loop)
	{
		if (!isNotPlay)
		{
			vl -= 0.3f;
			if (vl <= 0f)
			{
				vl = 0.01f;
			}
			playSoundBGLoop(type, vl);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ playSoundBGLoop.
	/// </summary>
	public static void playSoundBGLoop(int id, float volume)
	{
		if (GameCanvas.isPlaySound)
		{
			if (id == SoundMn.AIR_SHIP)
			{
				playSound1(id, volume + 0.2f);
			}
			else if (!(SoundBGLoop == null) && !isPlayingSoundBG(id))
			{
				SoundBGLoop.GetComponent<AudioSource>().loop = true;
				SoundBGLoop.GetComponent<AudioSource>().clip = music[id];
				SoundBGLoop.GetComponent<AudioSource>().volume = volume;
				SoundBGLoop.GetComponent<AudioSource>().Play();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ sTopSoundBG.
	/// </summary>
	public static void sTopSoundBG(int id)
	{
		SoundBGLoop.GetComponent<AudioSource>().Stop();
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlayingSoundBG.
	/// </summary>
	public static bool isPlayingSoundBG(int id)
	{
		if (SoundBGLoop == null)
		{
			return false;
		}
		return SoundBGLoop.GetComponent<AudioSource>().isPlaying;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho load.
	/// </summary>
	public static void load(string filename, int pos)
	{
		__load(filename, pos);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _load.
	/// </summary>
	private static void _load(string filename, int pos)
	{
		if (status != 0)
		{
			Cout.LogError("CANNOT LOAD AUDIO " + filename + " WHEN LOADING " + filenametemp);
			return;
		}
		filenametemp = filename;
		postem = pos;
		status = 2;
		int i;
		for (i = 0; i < 100; i++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (i == 100)
		{
			Cout.LogError("TOO LONG FOR LOAD AUDIO " + filename);
			return;
		}
		Cout.Log("Load Audio " + filename + " done in " + i * 5 + "ms");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __load.
	/// </summary>
	private static void __load(string filename, int pos)
	{
		music[pos] = (AudioClip)Resources.Load(filename, typeof(AudioClip));
		GameObject.Find("Main Camera").AddComponent<AudioSource>();
		player[pos] = GameObject.Find("Main Camera");
	}

	/// <summary>
	/// Bắt đầu khởi chạy tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public static void start(float volume, int pos)
	{
		__start(volume, pos);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _start.
	/// </summary>
	public static void _start(float volume, int pos)
	{
		if (status != 0)
		{
			Debug.LogError("CANNOT START AUDIO WHEN STARTING");
			return;
		}
		volumetem = volume;
		postem = pos;
		status = 3;
		int i;
		for (i = 0; i < 100; i++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (i == 100)
		{
			Debug.LogError("TOO LONG FOR START AUDIO");
		}
		else
		{
			Debug.Log("Start Audio done in " + i * 5 + "ms");
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __start.
	/// </summary>
	public static void __start(float volume, int pos)
	{
		if (player != null && pos >= 0 && pos < player.Length && player[pos] != null)
		{
			player[pos].GetComponent<AudioSource>()?.PlayOneShot(music[pos], volume);
		}
	}

	/// <summary>
	/// Dừng tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public static void stop(int pos)
	{
		__stop(pos);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ _stop.
	/// </summary>
	public static void _stop(int pos)
	{
		if (status != 0)
		{
			Debug.LogError("CANNOT STOP AUDIO WHEN STOPPING");
			return;
		}
		postem = pos;
		status = 4;
		int i;
		for (i = 0; i < 100; i++)
		{
			Thread.Sleep(5);
			if (status == 0)
			{
				break;
			}
		}
		if (i == 100)
		{
			Debug.LogError("TOO LONG FOR STOP AUDIO");
		}
		else
		{
			Debug.Log("Stop Audio done in " + i * 5 + "ms");
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ __stop.
	/// </summary>
	public static void __stop(int pos)
	{
		if (player != null && pos >= 0 && pos < player.Length && player[pos] != null)
		{
			player[pos].GetComponent<AudioSource>()?.Stop();
		}
	}
}
