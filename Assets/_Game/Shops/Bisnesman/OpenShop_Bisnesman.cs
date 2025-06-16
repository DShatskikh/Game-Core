using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class OpenShop_Bisnesman : OpenShopBase<ShopPresenter_Bisnesman, ShopPresenter_Bisnesman.Factory>
    {
        [SerializeField]
        private ShopPresenter_Bisnesman.InitData _initData;

        private protected override void Binding()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
                inscriptionsContainer.Add(localizedString.Key, localizedString.Value);
            
            _diContainer.BindFactory<ShopPresenter_Bisnesman, ShopPresenter_Bisnesman.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _background);
        }
    }
}