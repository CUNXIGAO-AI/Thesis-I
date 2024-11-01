using System.Collections;
using System.Collections.Generic;
using MalbersAnimations.Scriptables;
using UnityEngine;

public class TapeManager : MonoBehaviour
{
    public float TransitionDuration = 2.0f;// 控制渐变时间（秒）
    public float musicTransitionDuration = 5.0f;// 控制音频音量渐变时间
    public float fogDelay = 0.5f;// 变化的延迟时间（秒）
    public float musicDelay = 0.5f;// 音频变化的延迟时间
    public Light targetLight;// 需要调整的灯光
    public AudioSource audioSource;// 需要调整的音频
    public AudioClip audioClip;
    private Coroutine fogCoroutine;
    private Coroutine lightCoroutine;
    private Coroutine musicCoroutine;
    
    public FloatVar gravity;
    public float minGravity = 9.8f;
    public float maxGravity = 35f;
    public float minFogIntensity = 0.03f;
    public float maxFogIntensity = 0.12f;
    
    // 增加一个公有的只读属性，用来表示外部访问状态
    public bool CanChangeFog { get; private set; } = true; // 初始状态为true
    
    // Start is called before the first frame update
    void Start()
    {
        if(gravity != null)
        {
            gravity.Value = maxGravity;
        }
        
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

        RenderSettings.fogDensity = maxFogIntensity;
        CanChangeFog = true;  // 初始状态
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPickedUpFog()
    {
        Debug.Log("拾起来了");
        // 启动协程，延迟后将雾效渐变至 0.04
        if (fogCoroutine != null)
        {
            StopCoroutine(fogCoroutine);
        }
        fogCoroutine = StartCoroutine(ChangeFogIntensityWithDelay(minFogIntensity, fogDelay));
    }
    public void OnDroppedFog()
    {
        Debug.Log("丢下了");
        // 启动协程，延迟后将雾效渐变至 1.12
        if (fogCoroutine != null)
        {
            StopCoroutine(fogCoroutine);
        }
        fogCoroutine = StartCoroutine(ChangeFogIntensityWithDelay(maxFogIntensity, fogDelay));
    }
    private IEnumerator ChangeFogIntensityWithDelay(float targetIntensity, float delay)
    {
        // 等待设定的延迟时间
        yield return new WaitForSeconds(delay);

        float startIntensity = RenderSettings.fogDensity;
        float elapsedTime = 0f;

        while (elapsedTime < TransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // 插值计算新的 fogDensity 值
            RenderSettings.fogDensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / TransitionDuration);
            yield return null;  // 等待下一帧
        }

        // 确保最后设置为目标值
        RenderSettings.fogDensity = targetIntensity;
        //检查fog状态
        if (RenderSettings.fogDensity == maxFogIntensity)
        {
            CanChangeFog = true;
        }
        else
        {
            CanChangeFog = false;
        }
        Debug.Log("CanChangeFog: "+CanChangeFog.ToString());
    }

    
    public void OnPickedUpLight()
    {
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }
        lightCoroutine = StartCoroutine(ChangeLightIntensityWithDelay(10f, fogDelay)); // 根据需求修改目标亮度
    }
    public void OnDroppedLight()
    {
        if (lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
        }
        lightCoroutine = StartCoroutine(ChangeLightIntensityWithDelay(2f, fogDelay)); // 根据需求修改目标亮度
    }
    private IEnumerator ChangeLightIntensityWithDelay(float targetIntensity, float delay)
    {
        yield return new WaitForSeconds(delay);

        float startIntensity = targetLight.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < TransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            // 插值计算新的灯光 intensity 值
            targetLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / TransitionDuration);
            yield return null;  // 等待下一帧
        }
        // 确保最后设置为目标值
        targetLight.intensity = targetIntensity;
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
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }
        musicCoroutine = StartCoroutine(ChangeMusicVolumeWithDelay(1f, musicDelay)); // 音量渐入至 1
    }
    public void OnDroppedMusic()
    {
        Debug.Log("音频渐出");
        // 延迟后将音频音量渐变至 0
        if (musicCoroutine != null)
        {
            StopCoroutine(musicCoroutine);
        }
        musicCoroutine = StartCoroutine(ChangeMusicVolumeWithDelay(0f, musicDelay)); // 音量渐出至 0
    }
    private IEnumerator ChangeMusicVolumeWithDelay(float targetVolume, float delay)
    {
        yield return new WaitForSeconds(delay);

        float startVolume = audioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < musicTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / musicTransitionDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }


    public void OnPickedUpGravity()
    {
        gravity.Value = minGravity;
    }
    
    public void OnDroppedGravity()
    {
        gravity.Value = maxGravity;
    }
}
