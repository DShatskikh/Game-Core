using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game
{
    public class ShopButton : Button
    {
        [FormerlySerializedAs("_label")]
        [SerializeField]
        private TMP_Text getLabel;
        
        public TMP_Text GetLabel => getLabel;
        public event Action OnSelectAction;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectAction?.Invoke();
        }
    }
}