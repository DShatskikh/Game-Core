using UnityEngine;

namespace Game
{
    // Лестница
    public sealed class Ladder : MonoBehaviour
    {
        [SerializeField]
        private Transform _point1;

        [SerializeField]
        private Transform _point2;

        private IPlayerMover _previousMover;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                var isRight = Vector2.Distance(player.transform.position, _point2.position) >
                              Vector2.Distance(player.transform.position, _point1.position);
                
                _previousMover = player.GetMover;
                player.SetMover(new PlayerLadderMover(player.transform, _point2.position, _point1.position, isRight));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player)) 
                player.SetMover(_previousMover);
        }
    }
}