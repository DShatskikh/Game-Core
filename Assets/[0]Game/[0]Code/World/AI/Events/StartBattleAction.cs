using Game;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StartBattle", story: "Start [Battle]", category: "Action/Enemy", id: "1b1c33ecb0b64bfc0a0a5aa69ddad0e3")]
public partial class StartBattleAction : Action
{
    [SerializeReference]
    public BlackboardVariable<StarterBattleBase> Battle;

    protected override Status OnStart()
    {
        Battle.Value.StartBattle();
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

