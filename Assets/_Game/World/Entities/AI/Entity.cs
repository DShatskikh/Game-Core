using PixelCrushers.DialogueSystem;
using Unity.Behavior;
using UnityEngine;

namespace Game
{
    // Класс NPC
    public sealed class Entity : MonoBehaviour, IGameCutsceneListener
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        private StartDialogue _startDialogue;
        private ExitDialogue _exitDialogue;
        private Usable _usable;
        private bool _isOpenedDialogue;
        private BehaviorGraphAgent _behaviourAgent;
        public bool GetFlip => _spriteRenderer.flipX;

        private void Start()
        {
            _behaviourAgent = GetComponent<BehaviorGraphAgent>();
            
            if (_behaviourAgent.BlackboardReference.GetVariableValue("StartDialogueEvent", 
                    out StartDialogue startDialogue))
                _startDialogue = startDialogue;
            
            if (_behaviourAgent.BlackboardReference.GetVariableValue("ExitDialogueEvent", 
                    out ExitDialogue exitDialogue))
                _exitDialogue = exitDialogue;
            
            _usable = GetComponentInChildren<Usable>();
            _usable.events.onUse.AddListener(OnUse);
        }

        private void OnDestroy()
        {
            _usable.events.onUse.RemoveListener(OnUse);
        }

        private void OnUse()
        {
            _startDialogue.SendEventMessage();
            _isOpenedDialogue = true;
        }

        public void OnShowCutscene()
        {
            
        }

        public void OnHideCutscene()
        {
            if (_isOpenedDialogue)
            {
                _isOpenedDialogue = false;
                _exitDialogue.SendEventMessage();
            }
        }

        public void Flip(bool value)
        {
            _spriteRenderer.flipX = value;
        }
    }
}