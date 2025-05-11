using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/StartDialogue")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "StartDialogue", message: "Start Dialogue", category: "Events", id: "8d09558f9a2249d03f444198bbfb404a")]
public partial class StartDialogue : EventChannelBase
{
    public delegate void StartDialogueEventHandler();
    public event StartDialogueEventHandler Event; 

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
        StartDialogueEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as StartDialogueEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as StartDialogueEventHandler;
    }
}

