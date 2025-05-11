using Game;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FlipTo", story: "Flip [Entity] to [bool]", category: "Action/Enemy", id: "615a6814aa7361cf86e95e81739eac46")]
public partial class FlipToAction : Action
{
    [SerializeReference]
    public BlackboardVariable<Entity> Entity;
    
    [SerializeReference]
    public BlackboardVariable<bool> Bool;

    protected override Status OnStart()
    {
        var entity = Entity.Value;
        entity.Flip(Bool);
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

