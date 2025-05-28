using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Spawn_2_Tutorial : MonoBehaviour
    {
        [SerializeField]
        private GameObject _arrowBuySword;
        
        [SerializeField]
        private OpenShop_Notch_Tutorial _shopNotchTutorial;

        [SerializeField]
        private OpenShop_Notch _openShopNotch;

        [SerializeField]
        private UseObjectAction _notMoney;
        
        private TutorialState _tutorialState;

        [Inject]
        private void Construct(TutorialState tutorialState)
        {
            _tutorialState = tutorialState;
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

        private void OnStepStarted(TutorialStep currentStep)
        {
            Debug.Log(currentStep);
            _arrowBuySword.SetActive(false);
            
            if (currentStep == TutorialStep.BUY_SWORD)
            {
                _arrowBuySword.SetActive(true);
                _openShopNotch.enabled = false;
                GetComponent<Collider2D>().enabled = false;
                _shopNotchTutorial.gameObject.SetActive(true);
            }

            if (currentStep == TutorialStep.MOVE_TO_SHOP)
            {
                _arrowBuySword.SetActive(true);
                _openShopNotch.enabled = false;
                GetComponent<Collider2D>().enabled = false;
                _notMoney.gameObject.SetActive(true);
                _notMoney.GetEvent.AddListener(() =>
                {
                    _tutorialState.FinishStep();
                });
            }

            if (currentStep == TutorialStep.MOVE_MOB_ARENA)
            {
                Debug.Log("213");
            }
        }

        private void OnStepFinished(TutorialStep currentStep)
        {
            _arrowBuySword.SetActive(false);
            _shopNotchTutorial.gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = true;
            _openShopNotch.enabled = true;
            _notMoney.gameObject.SetActive(false);

            if (currentStep == TutorialStep.MOVE_TO_SHOP)
            {
                
            }
        }
    }
}