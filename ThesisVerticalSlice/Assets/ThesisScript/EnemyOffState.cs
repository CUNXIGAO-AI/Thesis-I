using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOffState : EnemyBaseState
{
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered OffState");
        enemy.StartCoroutine(DimLight(enemy.alertSpotLight));
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // 这里无需其他逻辑
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        Debug.Log("Exiting OffState.");
    }

    private IEnumerator DimLight(Light light) 
    {
        if (light == null)
        {
            Debug.LogWarning("No SpotLight found. Skipping dimming process.");
            yield break;
        }

        float duration = 3.0f; // 持续时间（秒）
        float startIntensity = light.intensity;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            light.intensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / duration);
            elapsedTime += Time.deltaTime; // 累加经过的时间
            yield return null; // 等待下一帧
        }

        light.intensity = 0f; // 确保最终亮度为 0
        Debug.Log("Light is completely dimmed.");
    }
}
