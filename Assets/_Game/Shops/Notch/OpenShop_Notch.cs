using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    // Класс открытия магазина Нотча
    public class OpenShop_Notch : OpenShopBase<ShopPresenter_Notch, ShopPresenter_Notch.Factory>
    {
        [SerializeField]
        private ShopPresenter_Notch.InitData _initData;

        private protected override void Binding()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
                inscriptionsContainer.Add(localizedString.Key, localizedString.Value);
            
            _diContainer.BindFactory<ShopPresenter_Notch, ShopPresenter_Notch.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _background);
        }
    }
}