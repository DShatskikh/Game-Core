using System;
using UnityEngine;

namespace Game
{
    public sealed class ArrowPointer : MonoBehaviour
    {
        [SerializeField]
        private GameObject _uiArrow;

        private Camera _cameraToCheck; // Укажите камеру в инспекторе или используйте Camera.main
        private GameObject _targetObject; // Пустой объект для проверки

        private void Start()
        {
            _targetObject = gameObject;
            _cameraToCheck = Camera.main;
        }

        private void Update()
        {
            if (IsVisible(_targetObject, _cameraToCheck))
            {
                _uiArrow.SetActive(false);
            }
            else
            {
                _uiArrow.SetActive(true);
            }
        }

        bool IsVisible(GameObject obj, Camera camera)
        {
            // Получаем позицию объекта в viewport пространстве (0-1)
            Vector3 viewportPosition = camera.WorldToViewportPoint(obj.transform.position);
        
            // Проверяем, находится ли позиция в пределах viewport (0-1 по X и Y)
            // И что объект перед камерой (Z > 0)
            return (viewportPosition.x >= 0 && viewportPosition.x <= 1 && 
                    viewportPosition.y >= 0 && viewportPosition.y <= 1 && 
                    viewportPosition.z > 0);
        }
    }
}