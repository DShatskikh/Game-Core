using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class ShopNotch : MonoBehaviour, IUseObject
    {
        [SerializeField]
        private SpriteRenderer _notch;
        
        [Header("Tutorial")]
        [SerializeField]
        private DialogueSystemTrigger _firstDialogue;
        
        [SerializeField]
        private DialogueSystemTrigger _secondDialogue;
    
        [SerializeField]
        private DialogueSystemTrigger _notMoneyDialogue;
        
        [SerializeField]
        private DialogueSystemTrigger _notMoneySecondDialogue;
        
        [SerializeField]
        private OpenShop_Notch _openShopNotch;

        [SerializeField]
        private GameObject _arrowBuySword;
        
        [SerializeField]
        private OpenShop_Notch_Tutorial _shopNotchTutorial;
        
        private TutorialState _tutorialState;
        private IGameRepositoryStorage _gameRepositoryStorage;
        private Player _player;
        private bool _isStartFlipX;

        [Inject]
        private void Construct(TutorialState tutorialState, IGameRepositoryStorage gameRepositoryStorage, Player player)
        {
            _tutorialState = tutorialState;
            _gameRepositoryStorage = gameRepositoryStorage;
            _player = player;

            _isStartFlipX = _notch.flipX;
        }

        private void Start()
        {
            _tutorialState.OnStepStarted += OnStepStarted;
            _tutorialState.OnStepFinished += OnStepFinished;

            OnStepStarted(_tutorialState.CurrentStep);
        }
        
        private void OnDestroy()
        {
            _tutorialState.OnStepStarted -= OnStepStarted;
            _tutorialState.OnStepFinished -= OnStepFinished;
        }
        
        private void Update()
        {
            if (Vector2.Distance(_player.transform.position, _notch.transform.position) < 5)
            {
                _notch.flipX = _player.transform.position.x < _notch.transform.position.x;
            }
            else
            {
                _notch.flipX = _isStartFlipX;
            }
        }

        public void Use()
        {
            if (_tutorialState.CurrentStep <= TutorialStep.BATTLE_BANANA)
            {
                if (!_gameRepositoryStorage.TryGet(SaveConstants.SHOP_NOTCH_FIRST_DIALOGUE, out MarkerData _))
                {
                    _firstDialogue.OnUse();
                    _gameRepositoryStorage.Set(SaveConstants.SHOP_NOTCH_FIRST_DIALOGUE, new MarkerData());
                }
                else
                {
                    _secondDialogue.OnUse();
                }
                
                return;
            }

            if (_tutorialState.CurrentStep == TutorialStep.MOVE_TO_SHOP_NO_MONEY_BUY_SWORD)
            {
                _notMoneyDialogue.OnUse();
                _tutorialState.FinishStep();
                return;
            }
            
            if (_tutorialState.CurrentStep == TutorialStep.MOVE_MOB_ARENA)
            {
                _notMoneySecondDialogue.OnUse();
                return;
            }

            if (_tutorialState.CurrentStep == TutorialStep.MOVE_TO_SHOP_BUY_SWORD)
            {
                _shopNotchTutorial.Open();
                return;
            }
            
            _openShopNotch.Open();
        }
        
        private void OnStepStarted(TutorialStep currentStep)
        {
            if (currentStep == TutorialStep.MOVE_TO_SHOP_NO_MONEY_BUY_SWORD)
            {
                _arrowBuySword.SetActive(true);
            }

            if (currentStep == TutorialStep.MOVE_TO_SHOP_BUY_SWORD)
            {
                _arrowBuySword.SetActive(true);
            }
        }

        private void OnStepFinished(TutorialStep currentStep)
        {
            _arrowBuySword.SetActive(false);

            if (currentStep == TutorialStep.MOVE_TO_SHOP_NO_MONEY_BUY_SWORD)
            {
                _arrowBuySword.SetActive(false);
            }

            if (currentStep == TutorialStep.MOVE_TO_SHOP_BUY_SWORD)
            {
                _arrowBuySword.SetActive(false);
            }
        }
    }
}