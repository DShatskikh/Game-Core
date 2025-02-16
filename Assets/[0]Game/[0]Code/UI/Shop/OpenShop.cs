using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class OpenShop : MonoBehaviour
    {
        [SerializeField]
        private ShopView _shopViewPrefab;

        [SerializeField]
        private ShopButton _prefab;

        [SerializeField]
        private ShopPresenter.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private AudioClip _music;
        
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
            
            _diContainer.BindFactory<ShopPresenter, ShopPresenter.ShopPresenterFactory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _music);

            var factory = _diContainer.TryResolve<ShopPresenter.ShopPresenterFactory>();
            factory.Create();
            _diContainer.Unbind<ShopPresenter.ShopPresenterFactory>();
        }
    }
}