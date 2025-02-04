using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class TransitionTrigger : MonoBehaviour
    {
        [SerializeField]
        private string _nextLocationIndex = "World";

        [SerializeField]
        private int _pointIndex;
        
        [SerializeField]
        private AudioClip _audioClip;

        private TransitionService _transitionService;

        [Inject]
        private void Construct(TransitionService transitionService)
        {
            _transitionService = transitionService;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _transitionService.Transition(_nextLocationIndex, _pointIndex, _audioClip);
            }
        }
    }
}