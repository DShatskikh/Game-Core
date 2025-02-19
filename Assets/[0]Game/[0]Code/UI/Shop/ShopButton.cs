using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class ShopButton : Button
    {
        [SerializeField]
        private TMP_Text _label;
        
        public TMP_Text GetLabel => _label;
        public event Action OnSelectAction;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectAction?.Invoke();
        }
        
        public void Init(string text, UnityAction action)
        {
            _label.text = text;
            onClick.AddListener(action);
        }
    }
}