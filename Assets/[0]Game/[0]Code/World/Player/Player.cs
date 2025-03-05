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

        [SerializeField]
        private CinemachineCamera _cinemachineCamera;
        
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
        private CameraSpeedSize _cameraSpeedSize;

        public IPlayerMover GetMover => _mover;
        public IReadOnlyReactiveProperty<float> GetCurrentSpeed => _currentSpeed;
        
        public IReadOnlyReactiveProperty<MonoBehaviour> NearestUseObject => 
            _useAreaChecker.NearestUseObject;

        [Inject]
        private void Construct(CinemachineConfiner2D confiner, PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _rigidbody = GetComponent<Rigidbody2D>();
            _mover = new PlayerMover(_rigidbody);
            _cameraAreaChecker = new CameraAreaChecker(transform, _cameraLayer, confiner);

            _cameraSpeedSize = new CameraSpeedSize(_cinemachineCamera);

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
            ToggleActivate(true);
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
            ToggleActivate(false);
        }

        void IGameResumeListener.OnResumeGame()
        {
            ToggleActivate(true);
        }

        void IGameTransitionListener.OnStartTransition()
        {
            ToggleActivate(false);
        }

        void IGameTransitionListener.OnEndTransition()
        {
            ToggleActivate(true);
        }

        void IGameShopListener.OnOpenShop()
        {
            ToggleActivate(false);
        }

        void IGameShopListener.OnCloseShop()
        {
            ToggleActivate(true);
        }

        void IGameADSListener.OnShowADS()
        {
            ToggleActivate(false);
        }

        void IGameADSListener.OnHideADS()
        {
            ToggleActivate(true);
        }

        void IGameDialogueListener.OnShowDialogue()
        {
            ToggleActivate(false);
        }

        void IGameDialogueListener.OnHideDialogue()
        {
            ToggleActivate(true);
        }

        void IGameEnderChestListener.OnOpenEnderChest()
        {
            ToggleActivate(false);
        }

        void IGameEnderChestListener.OnCloseEnderChest()
        {
            ToggleActivate(true);
        }
        
        public void OnOpenBattle()
        {
            ToggleActivate(false);
        }

        public void OnCloseBattle()
        {
            ToggleActivate(true);
        }
        
        private void ToggleActivate(bool isActivate)
        {
            _isPause = !isActivate;

            if (isActivate)
            {
                _previousPosition = transform.position;

                _currentSpeed = new ReactiveProperty<float>();
                _isRun = new ReactiveProperty<bool>();
                _direction = new ReactiveProperty<Vector2>();
                
                _currentSpeed.SubscribeAndCall(_stepsSoundPlayer.OnSpeedChange);
                _isRun.SubscribeAndCall(_stepsSoundPlayer.OnIsRunChange);
                _isRun.SubscribeAndCall(_view.OnIsRunChange);
                _direction.SubscribeAndCall(_view.OnDirectionChange);
                _currentSpeed.SubscribeAndCall(_view.OnSpeedChange);
                _currentSpeed.SubscribeAndCall(_cameraSpeedSize.OnChangeSpeed);
                _useAreaChecker.Lost();
                _playerInput.actions["Move"].canceled += OnInputMove;
                _playerInput.actions["Submit"].canceled += OnSubmit;
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
                _playerInput.actions["Submit"].canceled -= OnSubmit;
            }
        }

        private void OnSubmit(InputAction.CallbackContext obj)
        {
            _useAreaChecker.Use();
        }

        private void OnInputMove(InputAction.CallbackContext obj)
        {
            _mover.Stop();
        }

        public void PlaySwordAttack()
        {
            _view.SwordAttack();
        }
    }
}