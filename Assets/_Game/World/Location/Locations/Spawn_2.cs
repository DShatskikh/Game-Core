using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Game
{
    public sealed class Spawn_2 : MonoBehaviour
    {
        [SerializeField]
        private bool _isStart;

        [SerializeField]
        private DialogueSystemTrigger _dialogue;
        
        private void Start()
        {
            if (!_isStart)
                return;
            
            _dialogue.OnUse();
        }

        public void MoveToNotch()
        {
            Debug.Log("1");
            StartCoroutine(AwaitMoveToNotch());
        }
        
        private IEnumerator AwaitMoveToNotch()
        {
            Debug.Log("2");
            yield return new WaitForSeconds(1);
            Debug.Log("3");
            Sequencer.Message("MovedToNotch");
        }
    }
}