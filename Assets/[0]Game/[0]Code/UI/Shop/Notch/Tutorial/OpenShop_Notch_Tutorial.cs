using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class OpenShop_Notch_Tutorial : OpenShopBase<ShopPresenter_Notch_Tutorial, ShopPresenter_Notch_Tutorial.Factory>
    {
        [SerializeField]
        private ShopPresenter_Notch_Tutorial.InitData _initData;
        
        private protected override void Binding()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
                inscriptionsContainer.Add(localizedString.Key, localizedString.Value);
            
            _diContainer.BindFactory<ShopPresenter_Notch_Tutorial, ShopPresenter_Notch_Tutorial.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _background);
        }
    }
}