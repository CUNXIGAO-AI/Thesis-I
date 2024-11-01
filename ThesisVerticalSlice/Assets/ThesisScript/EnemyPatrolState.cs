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
        Debug.Log("Start from Patrol State");
        navAgent = enemy.GetComponent<NavMeshAgent>();
        navAgent.speed = enemy.patrolSpeed;
    }

    public override void UpdateState(EnemyStateManager enemy)
    {
        Patrol(enemy);
    }

    public override void ExitState(EnemyStateManager enemy)
    {
        // 离开巡逻状态时执行的逻辑
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
