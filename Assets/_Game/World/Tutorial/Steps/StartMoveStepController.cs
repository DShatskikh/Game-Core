using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartMoveStepController : BaseStepController
    {
        [SerializeField]
        private GameObject _hint;
        
        private Player _player;

        private protected override TutorialStep _step => TutorialStep.START_MOVE;

        [Inject]
        private void Construct(Player player)
        {
            _player = player;
        }

        private protected override void OnStepStarted()
        {
            _hint.SetActive(true);
            StartCoroutine(AwaitProcess());
        }

        private protected override void OnStepFinished()
        {
            _hint.SetActive(false);
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