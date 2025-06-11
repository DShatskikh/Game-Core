using Game.Editor;
using UnityEngine;
using Zenject;

namespace Game
{
    // Триггер для перехода на другую локацию
    public sealed class TransitionLocationTrigger : MonoBehaviour
    {
        [LocationID] 
        [SerializeField]
        private string _id;

        [SerializeField]
        private int _pointIndex;
        
        private TransitionService _transitionService;

        [Inject]
        private void Construct(TransitionService transitionService)
        {
            _transitionService = transitionService;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
                Transition();
        }

        public void Transition()
        {
            _transitionService.Transition(_id, _pointIndex);
        }
    }
}