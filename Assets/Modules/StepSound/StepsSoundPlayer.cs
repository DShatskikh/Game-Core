using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace StepSound
{
    [Serializable]
    public sealed class StepsSoundPlayer
    {
        private const float INTERVAL_STEP = 0.7f;

        [Header("Components")]
        [SerializeField] 
        private AudioSource _stepSource1;

        [SerializeField] 
        private AudioSource _stepSource2;

        [Header("Settings")]
        [SerializeField]
        private LayerMask _layerMask;

        [Header("Configs")]
        [SerializeField]
        private StepSoundPairsConfig _config;

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

        public void OnSpeedChange(float value)
        {
            if (value == 0)
                return;
            
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
            var pair = _tileTagConfig.GetPair(tile, _config);
            
            if (_isStepRight)
            {
                AudioClip clipToPlay = pair.Right;
                _stepSource1.clip = clipToPlay;
                _stepSource1.Play();
            }
            else
            {
                AudioClip clipToPlay = pair.Left;
                _stepSource2.clip = clipToPlay;
                _stepSource2.Play();   
            }
        }
    }
}