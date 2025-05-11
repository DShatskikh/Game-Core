using System;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game
{
    public sealed class Lever : MonoBehaviour, IUseObject
    {
        [Serializable]
        public struct Data
        {
            public bool IsActivated;

            public Data(bool isActivated)
            {
                IsActivated = isActivated;
            }
        }
        
        [SerializeField]
        private Sprite _activatedSprite;

        [SerializeField]
        private bool _isWrite;

        [ShowIf("_isWrite"), SerializeField]
        private string _saveKey;

        [SerializeField]
        private StudioEventEmitter _activatedSound;

        [SerializeField]
        private StudioEventEmitter _breakSound;
        
        private bool _isActivated;
        private SpriteRenderer _spriteRenderer;
        private Sprite _startSprite;
        private Collider2D _collider;
        public event Action OnUsable;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _startSprite = _spriteRenderer.sprite;
        }

        private void Start()
        {
            if (_isWrite && !_saveKey.IsNullOrWhitespace())
            {
                _isActivated = RepositoryStorage.Get<Data>(_saveKey).IsActivated;

                if (_isActivated)
                {
                    ActivateNoAnimation();
                }
            }
        }

        public void Use()
        {
            _isActivated = true;
            _collider.enabled = false;
            _spriteRenderer.sprite = _activatedSprite;

            if (_isWrite && !_saveKey.IsNullOrWhitespace()) 
                RepositoryStorage.Set(_saveKey, new Data(true));

            _activatedSound.Play();
            OnUsable?.Invoke();
        }

        public void Reset()
        {
            _spriteRenderer.sprite = _startSprite;
            _collider.enabled = true;
            _isActivated = false;
            _breakSound.Play();
        }

        public void ActivateNoAnimation()
        {
            _isActivated = true;
            _collider.enabled = false;
            _spriteRenderer.sprite = _activatedSprite;
        }
    }
}