using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/ExitDialogue")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "ExitDialogue", message: "Exit Dialogue", category: "Events", id: "6ad4ff6090997eb3e366fdf186339e03")]
public partial class ExitDialogue : EventChannelBase
{
    public delegate void ExitDialogueEventHandler();
    public event ExitDialogueEventHandler Event; 

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
        ExitDialogueEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as ExitDialogueEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as ExitDialogueEventHandler;
    }
}

