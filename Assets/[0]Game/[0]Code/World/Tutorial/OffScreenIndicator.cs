using System;
using UnityEngine;

namespace Game
{
    public class OffScreenIndicator : MonoBehaviour
    {
        [Header("Settings")]
        public Transform target;          // Целевой объект, за которым следим
        public RectTransform indicator;   // UI стрелка-индикатор
        public float edgeOffset = 50f;    // Отступ от краев экрана
        public float rotationSpeed = 5f;  // Скорость поворота стрелки

        [Header("Screen Bounds")]
        public Canvas canvas;             // Основной канвас
        public Camera mainCamera;         // Основная камера

        private RectTransform canvasRect;
        private Vector3 screenCenter;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            UpdateArrowPosition();
        }

        private void UpdateArrowPosition()
        {
            Vector3 dir = (target.position - mainCamera.transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            target.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Размещаем стрелку у края экрана
            Vector3 screenPos = mainCamera.WorldToViewportPoint(target.position);
            screenPos.x = Mathf.Clamp(screenPos.x, 0.1f, 0.9f);
            screenPos.y = Mathf.Clamp(screenPos.y, 0.1f, 0.9f);
            target.position = mainCamera.ViewportToWorldPoint(screenPos);
        }
    }
}