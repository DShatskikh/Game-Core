using System;
using PixelCrushers.DialogueSystem;
using UniRx;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class UseAreaChecker
    {
        [SerializeField]
        private float _radius;

        [SerializeField]
        private Transform _point;

        private ReactiveProperty<MonoBehaviour> _nearestUseObject = new();
        public IReadOnlyReactiveProperty<MonoBehaviour> NearestUseObject => _nearestUseObject;

        public void Search()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(_point.position, _radius);

            float minDistance = float.MaxValue;
            MonoBehaviour nearestUseObject = null;

            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out IUseObject useObject))
                {
                    var currentDistance = Vector2.Distance(_point.position, ((MonoBehaviour)useObject).transform.position);
                    
                    if (minDistance > currentDistance)
                    {
                        minDistance = currentDistance;
                        nearestUseObject = (MonoBehaviour)useObject;
                    }
                }
                else if (collider.TryGetComponent(out Usable usable))
                {
                    var currentDistance = Vector2.Distance(_point.position, usable.transform.position);
                    
                    if (minDistance > currentDistance)
                    {
                        minDistance = currentDistance;
                        nearestUseObject = usable;
                    }
                }
            }

            if (nearestUseObject != null)
            {
                if (nearestUseObject != _nearestUseObject.Value)
                {
                    Found(nearestUseObject);
                }
            }
            else if (_nearestUseObject.Value)
                Lost();
        }

        public void Found(MonoBehaviour nearestUseObject)
        {
            _nearestUseObject.Value = nearestUseObject;
        }
        
        public void Lost()
        {
            _nearestUseObject.Value = null;
        }

        public void Use()
        {
            if (!_nearestUseObject.Value)
                return;

            switch (_nearestUseObject.Value)
            {
                case IUseObject useObject:
                    useObject.Use();
                    break;
                case Usable currentUsable:
                {
                    currentUsable.OnUseUsable();
                    if (currentUsable != null)
                        currentUsable.gameObject.BroadcastMessage("OnUse", _point,
                            SendMessageOptions.DontRequireReceiver);
                    break;
                }
            }
        }
    }
}