using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertState : EnemyBaseState
{
    // Start is called before the first frame update
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("Entered Alert State");
    }
    
    public override void UpdateState(EnemyStateManager enemy)
    {

    }
    public override void ExitState(EnemyStateManager enemy)
    {
    }
}
