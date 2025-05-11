using Game;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Object = UnityEngine.Object;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlipToPlayer", story: "Flip [Entity] to player return [StartFlip]", category: "Action/Enemy", id: "1cce10bb0f33756db922cb4edd905845")]
public partial class FlipToPlayerAction : Action
{
    [SerializeReference]
    public BlackboardVariable<Entity> Entity;
    
    [SerializeReference]
    public BlackboardVariable<bool> StartFlip;

    protected override Status OnStart()
    {
        var entity = Entity.Value;
        var player = Object.FindFirstObjectByType<Player>();
        
        StartFlip.Value = entity.GetFlip;
        entity.Flip(player.transform.position.x < entity.transform.position.x);
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

