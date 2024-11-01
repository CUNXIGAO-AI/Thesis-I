using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySusState : EnemyBaseState
{
    // Start is called before the first frame update
    public override void EnterState(EnemyStateManager enemy)
    {
        Debug.Log("entered sus state");
    }
    
    public override void UpdateState(EnemyStateManager enemy)
    {

    }
    public override void ExitState(EnemyStateManager enemy)
    {
    }
}
