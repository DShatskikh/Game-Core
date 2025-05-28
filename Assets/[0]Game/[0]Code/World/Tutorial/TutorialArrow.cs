using UnityEngine;

namespace Game
{
    public sealed class TutorialArrow : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        private void Update()
        {
            transform.position = transform.position.SetY(Camera.main.WorldToScreenPoint(_target.position).y);
        }
    }
}