using Unity.Cinemachine;
using UnityEngine;

namespace CameraAreaUtility
{
    public sealed class CameraAreaChecker
    {
        private readonly Transform _point;
        private readonly LayerMask _layerMask;
        private readonly CinemachineConfiner2D _confiner2D;

        public CameraAreaChecker(Transform point, LayerMask layerMask, CinemachineConfiner2D confiner2D)
        {
            _point = point;
            _layerMask = layerMask;
            _confiner2D = confiner2D;
        }

        public void Check()
        {
            RaycastHit2D hit = Physics2D.Raycast(_point.position, Vector2.down, 0.1f, _layerMask);
            CameraArea nearestUseObject = null;

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out CameraArea cameraArea))
                {
                    nearestUseObject = cameraArea;
                }
            }

            _confiner2D.BoundingShape2D = nearestUseObject
                ? nearestUseObject.GetComponent<PolygonCollider2D>()
                : null;
            
            _confiner2D.InvalidateLensCache();
        }
    }
}