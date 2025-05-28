using TMPro;
using UnityEngine;

namespace Game
{
    // Визуальная часть окна сообщений противника во время битвы
    public class BattleMessageBoxView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;

        public void SetText(string text) => 
            _label.text = text;

        public void ToggleActivate(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}