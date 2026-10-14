using UnityEngine;

/// <summary>
/// CLASS MyAudioClip: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class MyAudioClip
{
	public string name;

	public AudioClip clip;

	public long timeStart;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MyAudioClip.
	/// </summary>
	public MyAudioClip(string filename)
	{
		clip = (AudioClip)Resources.Load(filename);
		name = filename;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Play.
	/// </summary>
	public void Play()
	{
		Main.main.GetComponent<AudioSource>().PlayOneShot(clip);
		timeStart = mSystem.currentTimeMillis();
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPlaying.
	/// </summary>
	public bool isPlaying()
	{
		return false;
	}
}
