using UnityEngine;

public abstract class EnemyBaseState
{
    //this is an abstract class 当蓝图用的
    public abstract void EnterState(EnemyStateManager enemy);
    public abstract void UpdateState(EnemyStateManager enemy);
    public abstract void ExitState(EnemyStateManager enemy);

}
