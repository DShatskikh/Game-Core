using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/TriggerEnter")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "TriggerEnter", message: "Trigger enter", category: "Events", id: "ee43245ee970724bbbc090280d9e4d1a")]
public partial class TriggerEnter : EventChannelBase
{
    public delegate void TriggerEnterEventHandler();
    public event TriggerEnterEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        TriggerEnterEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as TriggerEnterEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as TriggerEnterEventHandler;
    }
}

