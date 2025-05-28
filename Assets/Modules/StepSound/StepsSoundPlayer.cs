using System;
using FMODUnity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StepSound
{
    // Класс который воспроизводит звуки шагов
    [Serializable]
    public sealed class StepsSoundPlayer
    {
        private const float INTERVAL_STEP = 0.7f;

        [Header("Components")]
        [SerializeField] 
        private StudioEventEmitter _studioEventEmitter;
        
        [Header("Settings")]
        [SerializeField]
        private LayerMask _layerMask;

        [Header("Configs")]
        [SerializeField]
        private TileTagConfig _tileTagConfig;
        
        private bool _isStepRight;
        private float _currentStepTime;
        
        private Transform _transform;
        private bool _isRun;

        public void Init(Transform transform)
        {
            _transform = transform;
        }

        public void Upgrade()
        {
            _currentStepTime += Time.deltaTime;
                
            if (_currentStepTime >= (_isRun ? INTERVAL_STEP / 1.5f : INTERVAL_STEP))
            {
                _currentStepTime = 0;
                RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, 0.1f, _layerMask);
                TileBase tile = null;
                
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent(out Tilemap tilemap))
                    {
                        Vector3Int cellPosition = tilemap.WorldToCell(hit.point);
                        tile = tilemap.GetTile(cellPosition);
                    }
                }

                if (tile == null)
                    return;
                
                PlayFootstepSound(tile);
                _isStepRight = !_isStepRight;
            }
        }

        public void OnIsRunChange(bool value) => 
            _isRun = value;

        private void PlayFootstepSound(TileBase tile)
        {
            var pair = _tileTagConfig.GetPair(tile);
            _studioEventEmitter.Params[0].Value = pair;
            _studioEventEmitter.Play();
        }
    }
}