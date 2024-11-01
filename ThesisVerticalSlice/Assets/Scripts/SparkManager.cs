using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SparkManager : MonoBehaviour
{
    public float transitionDuration = 2.0f;// 控制音频音量渐变时间
    public float delay = 0.3f;// 音频变化的延迟时间
    public AudioSource audioSource;// 需要调整的音频
    public AudioClip audioClip;
    private Coroutine soundCoroutine;
    private Coroutine lightCoroutine;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // 确保音频剪辑已经被分配到 AudioSource
        if (audioSource != null && audioClip != null)
        {
            audioSource.clip = audioClip;
            // 预加载音频到内存
            audioSource.clip.LoadAudioData();
        }
        else
        {
            Debug.LogError("AudioSource 或 AudioClip 未正确分配。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnPickedUpMusic()
    {
        Debug.Log("音频渐入");
        // 如果音频尚未播放，则开始播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        // 延迟后将音频音量渐变至 1
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
        }
        soundCoroutine = StartCoroutine(ChangeMusicVolumeWithDelay(1f, delay)); // 音量渐入至 1
    }
    public void OnDroppedMusic()
    {
        Debug.Log("音频渐出");
        // 延迟后将音频音量渐变至 0
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
        }
        soundCoroutine = StartCoroutine(ChangeMusicVolumeWithDelay(0f, delay)); // 音量渐出至 0
    }
    private IEnumerator ChangeMusicVolumeWithDelay(float targetVolume, float delay)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / transitionDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
