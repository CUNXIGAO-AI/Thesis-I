using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearchState : EnemyBaseState
{
    // Start is called before the first frame update
    // Start is called before the first frame update
public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered Search State");

        // 初始化搜索旋转的目标
        if (enemy.searchRotationPoints.Length > 0)
        {
            enemy.searchTargetRotation = Quaternion.Euler(enemy.searchRotationPoints[enemy.searchRotationIndex]);
        }
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.canDecreaseAlertMeter = true;

        // 执行旋转逻辑
        PerformSearchRotation(enemy);
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // 离开搜索状态时清理逻辑（如果需要）
    }

    private static void PerformSearchRotation(EnemyStateManager enemy)
    {
        if (enemy.searchRotationPoints.Length == 0) return;

        // 平滑旋转到目标点
        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            enemy.searchTargetRotation,
            enemy.searchRotationSpeed * Time.deltaTime
        );

        // 检查是否到达目标角度
        if (Quaternion.Angle(enemy.transform.rotation, enemy.searchTargetRotation) < 0.1f)
        {
            enemy.searchWaitTimer += Time.deltaTime;

            // 如果等待时间已达到，更新到下一个点
            if (enemy.searchWaitTimer >= enemy.searchWaitTimeAtPoint)
            {
                enemy.searchWaitTimer = 0f;
                enemy.searchRotationIndex = (enemy.searchRotationIndex + 1) % enemy.searchRotationPoints.Length;
                enemy.searchTargetRotation = Quaternion.Euler(enemy.searchRotationPoints[enemy.searchRotationIndex]);
            }
        }
    }
}

