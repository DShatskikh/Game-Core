using System;
using UnityEngine;

namespace Game
{
    public sealed class HeartModeService
    {
        private Heart.Mode _mode;
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