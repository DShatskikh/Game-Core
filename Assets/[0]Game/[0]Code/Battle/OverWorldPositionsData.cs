using UnityEngine;

namespace Game
{
    public struct OverWorldPositionsData
    {
        public Transform Transform;
        public Transform Point;
        public Vector2 StartPosition;
        public Transform StartParent;

        public OverWorldPositionsData(Transform transform, Transform point)
        {
            Transform = transform;
            Point = point;
            StartPosition = transform.position;
            StartParent = transform.parent;
        }
    }
}