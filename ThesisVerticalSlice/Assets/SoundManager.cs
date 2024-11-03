using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Source for SFX")]
    public AudioSource glassBrokenSource;  // 在Inspector中设置的AudioSource用于播放音效

    [Header("Sound Clips")]
    public AudioClip glassBrokenClip;  // 用于存储玻璃破碎音效的 AudioClip

    [Header("Audio Sources")]
    public AudioSource loopingSFXSource;  // 用于循环播放的音效源

    [Header("Sound Clips")]
    public AudioClip depletionLoopClip;  // 循环播放的音效（资源耗减音效）

    private void Awake()
    {
        // Ensure only one instance of SoundManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Make this object persist across scenes
        }
        else
        {
            Destroy(gameObject);  // Destroy duplicate instances
        }
    }

    public void PlayGlassBrokenSound()
    {
        if (glassBrokenClip != null && glassBrokenSource != null)
        {
            glassBrokenSource.PlayOneShot(glassBrokenClip);
        }
        else
        {
            Debug.LogWarning("SoundManager: Glass broken sound clip or SFX source not set.");
        }
    }

     public void PlayLoopingSound()
    {
        if (loopingSFXSource != null && depletionLoopClip != null)
        {
            loopingSFXSource.clip = depletionLoopClip;
            loopingSFXSource.loop = true;  // 设置循环播放
            if (!loopingSFXSource.isPlaying)
            {
                loopingSFXSource.Play();
            }
        }
    }

    public void StopLoopingSound()
    {
        if (loopingSFXSource != null && loopingSFXSource.isPlaying)
        {
            loopingSFXSource.Stop();
        }
    }
}
