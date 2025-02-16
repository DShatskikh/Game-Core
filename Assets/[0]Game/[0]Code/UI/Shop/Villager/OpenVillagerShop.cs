using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class OpenVillagerShop : MonoBehaviour
    {
        [SerializeField]
        private ShopView _shopViewPrefab;

        [SerializeField]
        private ShopButton _prefab;

        [SerializeField]
        private VillagerShopPresenter.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private AudioClip _music;
        
        [SerializeField]
        private ShopBackground _background;
        
        private DiContainer _diContainer;

        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        private void Start()
        {
            Open();
        }

        [Button]
        private void Open() => 
            StartCoroutine(AwaitOpen());

        private IEnumerator AwaitOpen()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
            {
                yield return localizedString.Value.AwaitLoad(text => 
                    inscriptionsContainer.Add(localizedString.Key, text));
            }
            
            _diContainer.BindFactory<VillagerShopPresenter, VillagerShopPresenter.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _music, _background);

            var factory = _diContainer.TryResolve<VillagerShopPresenter.Factory>();
            factory.Create();
            _diContainer.Unbind<VillagerShopPresenter.Factory>();
        }
    }
}