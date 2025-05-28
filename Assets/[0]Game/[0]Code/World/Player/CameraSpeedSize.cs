using DG.Tweening;
using Unity.Cinemachine;

namespace Game
{
    // Класс меняющий размер камеры в зависимости от скорости персонажа
    public sealed class CameraSpeedSize
    {
        private const float DefaultSize = 8;
        private const float WalkSize = 8.5f;
        private const float RunSize = 9;

        private float _size;
        private Tweener _tween;
        
        private readonly CinemachineCamera _virtualCamera;

        public CameraSpeedSize(CinemachineCamera virtualCamera)
        {
            _virtualCamera = virtualCamera;
        }

        public void OnChangeSpeed(float value)
        {
            float size;
                
            if (value > 0.1f)
            {
                size = RunSize;
            }
            else if (value > 0.05f)
            {
                size = WalkSize;
            }
            else
            {
                size = DefaultSize; 
            }

            if (size != _size)
            {
                _tween?.Kill();
                _tween = DOTween.To(x => _virtualCamera.Lens.OrthographicSize = x, 
                    _virtualCamera.Lens.OrthographicSize, size, 0.5f);
            }
        }
    }
}