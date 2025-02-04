using CameraAreaUtility;
using StepSound;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game
{
    public sealed class Player : MonoBehaviour
    {
        [SerializeField]
        private PlayerView _view;

        [SerializeField]
        private StepsSoundPlayer _stepsSoundPlayer;
        
        [Header("CameraAreaChecker")]
        [SerializeField]
        private LayerMask _cameraLayer;
        
        private readonly ReactiveProperty<float> _currentSpeed = new();
        private readonly ReactiveProperty<bool> _isRun = new();
        private readonly ReactiveProperty<Vector2> _direction = new();
        
        private Rigidbody2D _rigidbody;
        private IPlayerMover _mover;
        private CameraAreaChecker _cameraAreaChecker;
        private PlayerInput _playerInput;
        private bool _isPause;
        private Vector3 _previousPosition;
        
        public IPlayerMover GetMover => _mover;

        [Inject]
        private void Construct(CinemachineConfiner2D confiner, PlayerInput playerInput)
        {
            _playerInput = playerInput;
            _rigidbody = GetComponent<Rigidbody2D>();
            _mover = new PlayerMover(_rigidbody);
            _cameraAreaChecker = new CameraAreaChecker(transform, _cameraLayer, confiner);
            
            _stepsSoundPlayer.Init(transform);

            Activate(true);
        }

        private void Update()
        {
            _isRun.Value = _playerInput.actions["Cancel"].IsPressed();

            if (_playerInput.actions["Move"].IsPressed())
            {
                _direction.Value = _playerInput.actions["Move"].ReadValue<Vector2>().normalized;
                _mover.Move(_direction.Value,  _isRun.Value);
            }
        }

        public void FixedUpdate()
        {
            var position = transform.position;
            _currentSpeed.Value = ((Vector2)(_previousPosition - position)).magnitude;
            _previousPosition = position;

            if (!_isPause)
            {
                //_useAreaChecker.Search();
            }
        }

        public void SetMover(IPlayerMover mover)
        {
            _mover = mover;
        }

        private void Activate(bool isActivate)
        {
            _isPause = !isActivate;

            if (isActivate)
            {
                _previousPosition = transform.position;

                _currentSpeed.Subscribe(_stepsSoundPlayer.OnSpeedChange);
                _isRun.Subscribe(_stepsSoundPlayer.OnIsRunChange);
                _direction.Subscribe(_view.OnDirectionChange);
                _currentSpeed.Subscribe(_view.OnSpeedChange);
                //_useAreaChecker.Lost();
                _playerInput.actions["Move"].canceled += OnInputMove;
            }
            else
            {
                _currentSpeed.Value = 0;
                _mover.Stop();
                //_useAreaChecker.Lost();
                
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