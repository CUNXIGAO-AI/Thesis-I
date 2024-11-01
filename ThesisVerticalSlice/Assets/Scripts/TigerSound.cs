using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TigerSound : MonoBehaviour
{
    // 声音渐变时间
    public float fadeDuration = 2f;
    // 音频组件
    public AudioSource _audioSource;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的物体是否带有"Animal"标签
        if (other.CompareTag("Animal"))
        {
            // 如果有正在进行的渐变协程，先停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            // 开始渐变到音量1
            fadeCoroutine = StartCoroutine(FadeAudioVolume(1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检查离开的物体是否带有"Animal"标签
        if (other.CompareTag("Animal"))
        {
            // 如果有正在进行的渐变协程，先停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            // 开始渐变到音量0
            fadeCoroutine = StartCoroutine(FadeAudioVolume(0f));
        }
    }

    private IEnumerator FadeAudioVolume(float targetVolume)
    {
        // 获取当前音量
        float startVolume = _audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // 逐渐改变音量
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保音量最终达到目标值
        _audioSource.volume = targetVolume;
    }
}
