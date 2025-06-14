using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Game
{
    public class OpenShop_Notch_Tutorial : OpenShopBase<ShopPresenter_Notch_Tutorial, ShopPresenter_Notch_Tutorial.Factory>
    {
        [SerializeField]
        private ShopPresenter_Notch_Tutorial.InitData _initData;

        [SerializeField]
        private DialogueSystemTrigger _enoughMoneyDialogue;
        
        public override void Open()
        {
            gameObject.SetActive(true);
            StartCoroutine(AwaitOpened());
        }
        
        private protected override void Binding()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
                inscriptionsContainer.Add(localizedString.Key, localizedString.Value);
            
            _diContainer.BindFactory<ShopPresenter_Notch_Tutorial, ShopPresenter_Notch_Tutorial.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _background);
        }

        private IEnumerator AwaitOpened()
        {
            _enoughMoneyDialogue.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            AwaitOpen().Forget();
        }
    }
}