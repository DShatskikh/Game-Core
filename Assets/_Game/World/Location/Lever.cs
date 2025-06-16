using System;
using FMODUnity;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Game
{
    // Рычаг
    public sealed class Lever : MonoBehaviour, IUseObject
    {
        // Структура данных для сохранения
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

        // Используем ли ключ сохранения
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
        private IGameRepositoryStorage _mainRepositoryStorage;
        public event Action OnUsable;

        [Inject]
        private void Construct(IGameRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }
        
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
                if (_mainRepositoryStorage.TryGet(_saveKey, out Data data))
                {
                    _isActivated = data.IsActivated;
                }
                
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
                _mainRepositoryStorage.Set(_saveKey, new Data(true));

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