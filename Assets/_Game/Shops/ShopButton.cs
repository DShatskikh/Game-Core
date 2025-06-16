using System;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    // Кнопка магазина
    public class ShopButton : Button
    {
        [SerializeField]
        private TMP_Text _label;
        
        [SerializeField]
        private StudioEventEmitter _selectSound;
        
        public TMP_Text GetLabel => _label;
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