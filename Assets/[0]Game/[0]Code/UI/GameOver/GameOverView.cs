using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class GameOverView : ScreenBase
    {
        [SerializeField]
        private Button _button;

        public void Show() => 
            gameObject.SetActive(true);
    }
}