using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Shop_Bisnesmen : MonoBehaviour, IUseObject
    {
        [SerializeField]
        private DialogueSystemTrigger _closeDialogue;
        
        [SerializeField]
        private OpenShop_Bisnesman _shopBisnesman;

        private TutorialState _tutorialState;

        [Inject]
        private void Construct(TutorialState tutorialState)
        {
            _tutorialState = tutorialState;
        }
        
        public void Use()
        {
            if (!_tutorialState.IsCompleted)
            {
                _closeDialogue.OnUse();
                return;
            }
            
            _shopBisnesman.Open();
        }
    }
}