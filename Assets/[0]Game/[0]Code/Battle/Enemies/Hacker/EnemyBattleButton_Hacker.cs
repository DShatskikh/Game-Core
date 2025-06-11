using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    // Переопределенная кнопка для противника Хакер
    public sealed class EnemyBattleButton_Hacker : EnemyBattleButton
    {
        [SerializeField]
        private Slider _healthMinusSlider;
        
        [SerializeField]
        private TMP_Text _healthLabel;
        
        public TMP_Text GetHealthLabel => _healthLabel;
        public Slider GetHealthMinusSlider => _healthMinusSlider;
    }
}