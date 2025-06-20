using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game
{
    public sealed class StartMoveStepController : BaseStepController
    {
        [FormerlySerializedAs("_hint")]
        [SerializeField]
        private GameObject _hintPC;
        
        [SerializeField]
        private GameObject _hintMobile;
        
        private Player _player;

        private protected override TutorialStep _step => TutorialStep.START_MOVE;

        [Inject]
        private void Construct(Player player)
        {
            _player = player;
        }

        private protected override void OnStepStarted()
        {
            Debug.Log("StartMoveStepController");
            
            var isMobile = false;

#if UNITY_WEBGL || UNITY_ANDROID
            if (DeviceTypeDetector.IsMobile())
            {
                isMobile = true;
            }
#endif
            
            if (isMobile)
            {
                _hintMobile.SetActive(true);
            }
            else
            {
                _hintPC.SetActive(true);
            }
            
            StartCoroutine(AwaitProcess());
        }

        private protected override void OnStepFinished()
        {
            _hintPC.SetActive(false);
            _hintMobile.SetActive(false);
        }

        private IEnumerator AwaitProcess()
        {
            var startDistance = _player.transform.position;
            yield return new WaitUntil(() => 
                Vector2.Distance(_player.transform.position, startDistance) > 1);
            _tutorialState.FinishStep();
        }
    }
}