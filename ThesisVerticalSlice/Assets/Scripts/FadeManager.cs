using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;  // TextMeshPro 的 Image 组件
    public float fadeDuration = 2f;    // 淡入持续时间

    private void Start()
    {
        // 开始时确保 Image 可见且完全不透明
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 1;  // 完全不透明
        fadeImage.color = color;
        
        // 开始淡入效果
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        // 逐渐将透明度从 1（不透明）调整到 0（完全透明）
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);  // 插值计算alpha
            Color color = fadeImage.color;
            color.a = alpha;  // 只改变 alpha 值
            fadeImage.color = color;
            yield return null;
        }

        // 最终完全透明并隐藏 Image
        Color finalColor = fadeImage.color;
        finalColor.a = 0;
        fadeImage.color = finalColor;
        fadeImage.gameObject.SetActive(false); // 隐藏 Image
    }
}
