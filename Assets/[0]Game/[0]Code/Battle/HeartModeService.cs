using System;
using UnityEngine;

namespace Game
{
    // Сервис управляющий состоянием сердца
    public sealed class HeartModeService
    {
        private Heart.Mode _mode = Heart.Mode.Blue;
        private readonly Sprite[] _icons;

        public Heart.Mode GetMode => _mode;
        public event Action<Heart.Mode> Upgrade;

        public HeartModeService(Sprite[] icons)
        {
            _icons = icons;
        }
        
        public void SetMode(Heart.Mode mode)
        {
            _mode = mode;
            Upgrade?.Invoke(mode);
        }

        // Метод получения иконки сердца в зависимости от текущего состояния
        public Sprite GetIcon()
        {
            switch (_mode)
            {
                case Heart.Mode.Red:
                    return _icons[0];
                case Heart.Mode.Blue:
                    return _icons[1];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}