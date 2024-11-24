using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : EnemyBaseState
{
    private int currentWaypointIndex = 0;  // 当前的巡逻点索引
    private NavMeshAgent navAgent;
    // Start is called before the first frame update

    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered Patrol State");
        navAgent = enemy.GetComponent<NavMeshAgent>();
        navAgent.speed = enemy.patrolSpeed;

        if (enemy.rotationPoints.Length > 0)
        {
            enemy.targetRotation = Quaternion.Euler(enemy.rotationPoints[enemy.currentRotationIndex]);
        }
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        Patrol(enemy);
        PerformRotation(enemy);
    
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // 离开巡逻状态时执行的逻辑
    }

    private static void PerformRotation(EnemyStateManager enemy) // 旋转
    {
        if (!enemy.shouldRotate || enemy.rotationPoints.Length == 0) return; // 如果不需要旋转或者没有旋转点，直接返回

        // 平滑旋转到目标点
        enemy.transform.rotation = Quaternion.RotateTowards(
            enemy.transform.rotation,
            enemy.targetRotation,
            enemy.rotationSpeed * Time.deltaTime
        );

        // 检查是否到达目标角度
        if (Quaternion.Angle(enemy.transform.rotation, enemy.targetRotation) < 0.1f)
        {
            enemy.waitTimer += Time.deltaTime;

            // 如果等待时间已达到，更新到下一个点
            if (enemy.waitTimer >= enemy.waitTimeAtPoint)
            {
                enemy.waitTimer = 0f;
                enemy.currentRotationIndex = (enemy.currentRotationIndex + 1) % enemy.rotationPoints.Length;
                enemy.targetRotation = Quaternion.Euler(enemy.rotationPoints[enemy.currentRotationIndex]);
            }
        }
    }
    private void Patrol(EnemyStateManager enemy)
    {
        if (enemy.waypoints.Length == 0) return;

        // 获取当前巡逻点
        Transform targetWaypoint = enemy.waypoints[currentWaypointIndex];

        // 设置 NavMeshAgent 的目标
        navAgent.SetDestination(targetWaypoint.position);

        // 检查是否接近目标巡逻点
        if (!navAgent.pathPending && navAgent.remainingDistance <= enemy.waypointReachThreshold)
        {
            // 切换到下一个巡逻点
            currentWaypointIndex = (currentWaypointIndex + 1) % enemy.waypoints.Length;
        }
    }
}
