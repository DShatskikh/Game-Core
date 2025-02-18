using TMPro;
using UnityEngine;

namespace Game
{
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