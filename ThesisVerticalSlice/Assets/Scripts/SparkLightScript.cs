using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkLightScript : MonoBehaviour
{
    public Light pointLight; // 点光源
    public float flickerPercentage = 0.5f; // 光强变化的百分比（0.2f 表示 20% 的变化）
    public float flickerSpeed = 10f; // 光强变化的速度
    public float rangeVariation = 5f; // 光照范围的变化幅度
    public float colorVariation = 0.1f; // 颜色变化的幅度

    private float baseIntensity;
    private float baseRange;
    private Color baseColor;

    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>(); // 获取点光源组件
        }

        // 保存初始的光强值
        baseIntensity = pointLight.intensity;
        // 保存原始的光照范围和颜色
        baseRange = pointLight.range;
        baseColor = pointLight.color;
    }

    void Update()
    {
        /// 使用 Perlin 噪声生成变化，使其变化平滑
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);

        // 基于当前光强度的百分比变化
        float intensityVariation = baseIntensity * flickerPercentage;

        // 计算新的光强度，范围为当前光强度的 ±flickerPercentage
        pointLight.intensity = baseIntensity + Mathf.Lerp(-intensityVariation, intensityVariation, noise);

        // 可选：增加光照范围的轻微波动
        pointLight.range = baseRange + Mathf.Sin(Time.time * flickerSpeed) * rangeVariation;

        // 可选：让光源颜色也微妙地变化，模拟火焰效果
        pointLight.color = baseColor + new Color(
            Random.Range(-colorVariation, colorVariation), 
            Random.Range(-colorVariation, colorVariation), 
            0f // 可以保持蓝色或绿色分量不变，模拟火焰的颜色
        );
    }
}
