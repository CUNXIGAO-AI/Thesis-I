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
    }
    
    public override void UpdateState(EnemyStateManager enemy)
    {
        enemy.canDecreaseAlertMeter = true;
    }

    public override void ExitState(EnemyStateManager enemy)
    {

    }
}

