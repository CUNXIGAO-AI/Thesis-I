using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatState : EnemyBaseState
{
    private float timeWithoutDetection = 0f; // 用于计时未检测到物品的时间
    private const float detectionTimeout = 5f; // 超时阈值（秒）

    private Vector3 currentOffset = Vector3.zero; // 当前的偏移量
    private Vector3 targetOffset = Vector3.zero; // 目标偏移量
    private const float offsetRange = 4f; // 偏移范围大小，如果是10的话感觉有个闪躲的gameplay
    private const float offsetSmoothSpeed = 3.5f; // 偏移更新的平滑速度

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered Combat State");
        enemy.BroadcastCombat();
        timeWithoutDetection = 0f; // 重置计时器
        enemy.playerLost = false;

        // 初始化偏移量
        UpdateTargetOffset();
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        if (enemy.item != null)
        {
            // 平滑更新偏移量
            currentOffset = Vector3.Lerp(currentOffset, targetOffset, Time.deltaTime * offsetSmoothSpeed);

            // 计算目标方向并应用偏移
            Vector3 directionToItem = (enemy.item.position - enemy.transform.position).normalized;
            Vector3 offsetDirection = directionToItem + currentOffset;

            Quaternion targetRotation = Quaternion.LookRotation(offsetDirection);
            float rotationSpeed = 5f;
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 每帧随机生成新的目标偏移方向
            UpdateTargetOffset();
        }

        // 检测物品状态
        if (!enemy.DetectItem())
        {
            timeWithoutDetection += Time.deltaTime;

            if (timeWithoutDetection >= detectionTimeout)
            {
                Debug.Log("Player Lost");
                enemy.playerLost = true;
                timeWithoutDetection = 0f; // 重置计时器
            }
        }
        else
        {
            timeWithoutDetection = 0f; // 重置计时器
            enemy.playerLost = false;
        }
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // 离开 CombatState 时的清理逻辑（如果需要）
    }

    // 随机更新目标偏移量
    private void UpdateTargetOffset() // 随机生成目标偏移量, 偏移量可以稍后更细致的调节
    {
        targetOffset = new Vector3(
            Random.Range(-offsetRange, offsetRange), // X 偏移
            Random.Range(-offsetRange, offsetRange), // Y 偏移
            Random.Range(-offsetRange, offsetRange)  // Z 偏移
        ).normalized * offsetRange;
    }

}
