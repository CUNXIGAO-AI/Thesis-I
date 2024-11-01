using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LightManager : MonoBehaviour
{
    // 目标亮度
    public float targetIntensity = 1000f;
    // 灯光渐变时间
    public float lightFadeDuration = 5f;
    // 灯光组件
    private Light _light;
    private Coroutine fadeCoroutine;
    // 是否灯光已经完全打开
    private bool lightIsOn = false;
    public TapeManager tapeManager;
    public float minFogIntensity = 0.03f;
    public float maxFogIntensity = 0.12f;
    public float TransitionDuration = 2.0f;// 控制渐变时间（秒）
    public float fogDelay = 0.3f;// 变化的延迟时间（秒）
    private Coroutine fogCoroutine;

    private void Start()
    {
        // 获取挂载的灯光组件
        _light = GetComponentInChildren<Light>();
        // 初始化灯光亮度为0
        _light.intensity = 0;
        lightIsOn = false; // 灯光初始状态为关闭
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入的物体是否带有"Spark"标签
        if (other.CompareTag("Spark"))
        {
            // 如果有正在进行的渐变协程，先停止
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            // 开始渐变到目标亮度
            fadeCoroutine = StartCoroutine(FadeLightIntensity(targetIntensity));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            if (lightIsOn && tapeManager.CanChangeFog)
            {
                // 启动协程，延迟后将雾效渐变至 0.04
                if (fogCoroutine != null)
                {
                    StopCoroutine(fogCoroutine);
                }
                fogCoroutine = StartCoroutine(ChangeFogIntensityWithDelay(minFogIntensity, fogDelay));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            if (lightIsOn && tapeManager.CanChangeFog)
            {
                // 启动协程，延迟后将雾效渐变至 0.04
                if (fogCoroutine != null)
                {
                    StopCoroutine(fogCoroutine);
                }
                fogCoroutine = StartCoroutine(ChangeFogIntensityWithDelay(maxFogIntensity, fogDelay));
            }
        }
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
            yield return null; // 等待下一帧
        }

        // 确保最后设置为目标值
        RenderSettings.fogDensity = targetIntensity;
    }

    private IEnumerator FadeLightIntensity(float target)
    {
        // 获取当前亮度
        float startIntensity = _light.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < lightFadeDuration)
        {
            // 逐渐改变亮度
            _light.intensity = Mathf.Lerp(startIntensity, target, elapsedTime / lightFadeDuration);
            
            // 当灯光亮度不为0时，lightIsOn为true，否则为false
            lightIsOn = _light.intensity > 0;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保最终亮度达到目标值
        _light.intensity = target;
        // 在协程结束时再次检查亮度
        lightIsOn = _light.intensity > 0;
        Debug.Log("LightIsOn: "+lightIsOn.ToString());
    }
}
