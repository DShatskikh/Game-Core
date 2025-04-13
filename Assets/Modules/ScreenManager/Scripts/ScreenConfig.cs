using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ScreenConfig", menuName = "Data/ScreenConfig", order = 200)]
    public sealed class ScreenConfig : ScriptableObject
    {
        [SerializeField]
        private ScreenData[] _data;
        
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
    
    [Serializable]
    public sealed class ScreenData
    {
        public ScreensEnum Enum;
        public ScreenBase Screen;

        [SerializeReference]
        public IScreenPresenter Presenter;
    }
}