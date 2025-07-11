﻿using UniRx;
using UnityEngine;

namespace Game
{
    // Хранилище прогресса битвы
    public sealed class TurnProgressStorage
    {
        private readonly ReactiveProperty<int> _progress = new();
        
        public IReadOnlyReactiveProperty<int> Progress => _progress;

        public void AddBattleProgress(int value)
        {
            _progress.Value += value;

            if (_progress.Value > 100) 
                _progress.Value = 100;
        }

        public void Reset() => 
            _progress.Value = 0;
    }
}