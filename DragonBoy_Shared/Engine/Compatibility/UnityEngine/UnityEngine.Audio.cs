using System;
using Raylib_cs;

namespace UnityEngine
{
    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp AudioClip: Đảm nhiệm phân hệ chuyên trách Audio ().
    /// </summary>
    public class AudioClip
    {
        public string name { get; set; } = string.Empty;
        public float length { get; set; } = 0f;
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp AudioSource: Đảm nhiệm phân hệ chuyên trách Audio ().
    /// </summary>
    public class AudioSource : Component
    {
        public AudioClip clip { get; set; }
        public float volume { get; set; } = 1.0f;
        public float pitch { get; set; } = 1.0f;
        public bool loop { get; set; } = false;
        public bool isPlaying { get; set; } = false;

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Play.
        /// </summary>
        public void Play()
        {
            isPlaying = true;
        }

        /// <summary>
        /// Dừng tiến trình hoặc luồng xử lý nền.
        /// </summary>
        public void Stop()
        {
            isPlaying = false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ Pause.
        /// </summary>
        public void Pause()
        {
            isPlaying = false;
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ PlayOneShot.
        /// </summary>
        public void PlayOneShot(AudioClip clip)
        {
            this.clip = clip;
            Play();
        }

        /// <summary>
        /// Thực thi phương thức nghiệp vụ PlayOneShot.
        /// </summary>
        public void PlayOneShot(AudioClip clip, float volumeScale)
        {
            this.clip = clip;
            this.volume = volumeScale;
            Play();
        }
    }

    /// <summary>
    /// Thành phần phân mảnh (partial) của lớp AudioListener: Đảm nhiệm phân hệ chuyên trách Audio ().
    /// </summary>
    public class AudioListener : Component
    {
        public static float volume { get; set; } = 1.0f;
    }
}
