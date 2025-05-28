using Game;
using System;
using System.Collections;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Random = UnityEngine.Random;

// Нода поведения которая ведет NPC в случайную точку
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "EnemyMoveRandomPoint", story: "[Enemy] to move random point", category: "Action/Enemy", id: "3c3b00c80ffb42580887ae9353b62400")]
public partial class EnemyMoveRandomPointAction : Action
{
    [SerializeReference]
    public BlackboardVariable<EnemyMover> Enemy;

    private bool _isStopMove;
    
    protected override Status OnStart()
    {
        _isStopMove = false;
        Enemy.Value.StartCoroutine(AwaitMoveRandom());
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return _isStopMove ? Status.Success :  Status.Running;
    }

    protected override void OnEnd()
    {
        Enemy.Value.StopMove();
    }

    private IEnumerator AwaitMoveRandom()
    {
        yield return new WaitForSeconds(Random.Range(0, 1f));
        
        var distance = 2;
        var target = (Vector2)Enemy.Value.transform.position
                     + new Vector2(1 * Random.Range(0, 2) == 0 ? 1 : -1,
                         1 * Random.Range(0, 2) == 0 ? 1 : -1)
                     * distance;
        Enemy.Value.MoveToPoint(target);
        yield return new WaitUntil(() => !Enemy.Value.IsMove);
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        _isStopMove = true;
    }
}

