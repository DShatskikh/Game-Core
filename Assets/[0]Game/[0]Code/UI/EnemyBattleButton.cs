using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public sealed class EnemyBattleButton : Button
    {
        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private TMP_Text _mercyLabel;
        
        [SerializeField]
        private Slider _mercySlider;
        
        [SerializeField]
        private Slider _healthSlider;

        [SerializeField]
        private StudioEventEmitter _selectSound;
        
        public TMP_Text GetLabel => _label;
        public TMP_Text GetMercyLabel => _mercyLabel;
        public Slider GetMercySlider => _mercySlider;
        public Slider GetHealthSlider => _healthSlider;
        public event Action OnSelectAction;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _selectSound.Play();
            OnSelectAction?.Invoke();
        }
        
        public void Init(string text, UnityAction action)
        {
            _label.text = text;
            onClick.AddListener(action);
        }
    }
}