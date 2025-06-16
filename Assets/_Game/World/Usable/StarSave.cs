using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

namespace Game
{
    // Звезда сохранений
    public sealed class StarSave : MonoBehaviour, IUseObject, IGameCutsceneListener
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        private bool _isOpened;
        private ScreenManager _screenManager;
        private DiContainer _diContainer;

        [Inject]
        private void Construct(ScreenManager screenManager, DiContainer diContainer)
        {
            _screenManager = screenManager;
            _diContainer = diContainer;
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
            _screenManager.Open(ScreensEnum.SAVE, _diContainer);
        }
    }
}