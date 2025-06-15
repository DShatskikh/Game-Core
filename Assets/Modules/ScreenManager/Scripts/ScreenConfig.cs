using System;
using UnityEngine;

namespace Game
{
    // Хранит конфигурацию окон
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "Data/ScreenConfig", order = 200)]
    public sealed class ScreenConfig : ScriptableObject
    {
        [SerializeField]
        private ScreenData[] _data;
        
        // Получить конфигурацию окна по индексу
        public bool TryGet(ScreensEnum screensEnum, out ScreenData result)
        {
            foreach (var data in _data)
            {
                if (data.Enum != screensEnum)
                    continue;
                
                result = data;
                return true;
            }
            
            result = null;
            return false;
        }
    }
    
    // Структура хранящая компоненты окон
    [Serializable]
    public sealed class ScreenData
    {
        public ScreensEnum Enum;
        public ScreenBase Screen;

        [SerializeReference]
        public IScreenPresenter Presenter;
    }
}

