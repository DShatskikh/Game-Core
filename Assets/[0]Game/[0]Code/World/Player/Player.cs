using CameraAreaUtility;
using StepSound;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public sealed class Player : MonoBehaviour, IGameStartListener, IGamePauseListener, IGameResumeListener, 
        IGameTickableListener,IGameFixedTickableListener, IGameTransitionListener,
        IGameShopListener, IGameADSListener, IGameDialogueListener, IGameEnderChestListener, IGameBattleListener
    {
        [SerializeField]
        private PlayerView _view;

        [SerializeField]
        private StepsSoundPlayer _stepsSoundPlayer;

        [SerializeField]
        private UseAreaChecker _useAreaChecker;
        
        [Header("CameraAreaChecker")]
        [SerializeField]
        private LayerMask _cameraLayer;
        
        private ReactiveProperty<float> _currentSpeed = new();
        private ReactiveProperty<bool> _isRun = new();
        private ReactiveProperty<Vector2> _direction = new();
        
        private Rigidbody2D _rigidbody;
        private IPlayerMover _mover;
        private CameraAreaChecker _cameraAreaChecker;
        private PlayerInput _playerInput;
        private bool _isPause;
        private Vector3 _previousPosition;
        
        public IPlayerMover GetMover => _mover;
        
        public IReadOnlyReactiveProperty<MonoBehaviour> NearestUseObject => 
            _useAreaChecker.NearestUseObject;

        [Inject]
        private void Construct(CinemachineConfiner2D confiner, PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _rigidbody = GetComponent<Rigidbody2D>();
            _mover = new PlayerMover(_rigidbody);
            _cameraAreaChecker = new CameraAreaChecker(transform, _cameraLayer, confiner);
            
            _stepsSoundPlayer.Init(transform);
        }

        public void FixedUpdate()
        {
            var position = transform.position;
            _currentSpeed.Value = ((Vector2)(_previousPosition - position)).magnitude;
            _previousPosition = position;

            if (!_isPause)
            {
                _useAreaChecker.Search();
            }
        }

        public void TryUse(InputAction.CallbackContext obj)
        {
            _useAreaChecker.Use();
        }
        
        public void SetMover(IPlayerMover mover)
        {
            _mover = mover;
        }

        void IGameStartListener.OnStartGame()
        {
            Activate(true);
        }

        void IGameTickableListener.Tick(float delta)
        {
            _isRun.Value = _playerInput.actions["Cancel"].IsPressed();

            if (_playerInput.actions["Move"].IsPressed())
            {
                _direction.Value = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
                _mover.Move(_direction.Value,  _isRun.Value);
            }
        }

        void IGameFixedTickableListener.FixedTick(float delta)
        {
            
        }

        void IGamePauseListener.OnPauseGame()
        {
            Activate(false);
        }

        void IGameResumeListener.OnResumeGame()
        {
            Activate(true);
        }

        void IGameTransitionListener.OnStartTransition()
        {
            Activate(false);
        }

        void IGameTransitionListener.OnEndTransition()
        {
            Activate(true);
        }

        void IGameShopListener.OnOpenShop()
        {
            Activate(false);
        }

        void IGameShopListener.OnCloseShop()
        {
            Activate(true);
        }

        void IGameADSListener.OnShowADS()
        {
            Activate(false);
        }

        void IGameADSListener.OnHideADS()
        {
            Activate(true);
        }

        void IGameDialogueListener.OnShowDialogue()
        {
            Activate(false);
        }

        void IGameDialogueListener.OnHideDialogue()
        {
            Activate(true);
        }

        void IGameEnderChestListener.OnOpenEnderChest()
        {
            Activate(false);
        }

        void IGameEnderChestListener.OnCloseEnderChest()
        {
            Activate(true);
        }
        
        public void OnOpenBattle()
        {
            Activate(false);
        }

        public void OnCloseBattle()
        {
            Activate(true);
        }
        
        private void Activate(bool isActivate)
        {
            _isPause = !isActivate;

            if (isActivate)
            {
                _previousPosition = transform.position;

                _currentSpeed = new ReactiveProperty<float>();
                _isRun = new ReactiveProperty<bool>();
                _direction = new ReactiveProperty<Vector2>();
                
                _currentSpeed.Subscribe(_stepsSoundPlayer.OnSpeedChange);
                _isRun.Subscribe(_stepsSoundPlayer.OnIsRunChange);
                _isRun.Subscribe(_view.OnIsRunChange);
                _direction.Subscribe(_view.OnDirectionChange);
                _currentSpeed.Subscribe(_view.OnSpeedChange);
                _useAreaChecker.Lost();
                _playerInput.actions["Move"].canceled += OnInputMove;
            }
            else
            {
                _currentSpeed.Value = 0;
                _mover.Stop();
                _useAreaChecker.Lost();
                
                _currentSpeed.Dispose();
                _isRun.Dispose();
                _direction.Dispose();
                
                //_currentSpeed.Changed -= _stepsSoundPlayer.OnSpeedChange;
                //_isRun.Changed -= _stepsSoundPlayer.OnIsRunChange;
                //_direction.Changed -= _view.OnDirectionChange;
                //_currentSpeed.Changed -= _view.OnSpeedChange;
                _playerInput.actions["Move"].canceled -= OnInputMove;
            }
        }

        private void OnInputMove(InputAction.CallbackContext obj)
        {
            _mover.Stop();
        }
    }
}