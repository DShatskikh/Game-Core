using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class DialogueExtensions
    {
        private static DialogueExtensions _instance;
        private Action _action;

        public void Init()
        {
            _instance = this;
        }

        public static void SubscriptionCloseDialog(Action action)
        {
            _instance._action += action;
        }
        
        public static IEnumerator AwaitCloseDialog()
        {
            bool isClose = false;

            SubscriptionCloseDialog(() =>
            {
                isClose = true;
            });
            
            yield return new WaitUntil(() => isClose);
        }

        private static void OnClose(Action action)
        {
            action.Invoke();
        }

        public void OnShowDialogue()
        {
            
        }

        public void OnHideDialogue()
        {
            _action?.Invoke();
            _action = null;
        }
    }
}