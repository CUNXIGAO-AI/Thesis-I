using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatState : EnemyBaseState
{
    // Start is called before the first frame update
 public override void EnterState(EnemyStateManager enemy)
    {
        // 在进入战斗状态时广播警报
        Debug.Log("Entereds Combat State");
        enemy.BroadcastAlert();
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        // 敌人看向玩家
        if (enemy.item != null)
        {
            Vector3 directionToPlayer = (enemy.item.position - enemy.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // 平滑旋转
            float rotationSpeed = 5f; // 增加旋转速度
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // 离开 CombatState 时的清理逻辑（如果需要）
    }

}
