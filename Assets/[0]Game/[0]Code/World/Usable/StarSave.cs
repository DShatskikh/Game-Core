using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StarSave : MonoBehaviour, IUseObject, IGameCutsceneListener
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        private bool _isOpened;
        private ScreenManager _screenManager;

        [Inject]
        private void Construct(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }
        
        public void Use()
        {
            _isOpened = true;
            _dialogueSystemTrigger.OnUse();
        }

        public void OnShowCutscene() { }

        public void OnHideCutscene()
        {
            if (!_isOpened)
                return;

            _isOpened = false;
            _screenManager.Open(ScreensEnum.SAVE);
        }
    }
}