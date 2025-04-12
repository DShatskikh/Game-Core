using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class OpenShop_Villager : MonoBehaviour
    {
        [SerializeField]
        private ShopView _shopViewPrefab;

        [SerializeField]
        private ShopButton _prefab;

        [SerializeField]
        private ShopPresenter_Villager.InitData _initData;

        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private StudioEventEmitter _studioEvent;
        
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
            //Open();
        }

        [Button]
        public void Open()
        {
            gameObject.SetActive(true);
            StartCoroutine(AwaitOpen());
        }

        private IEnumerator AwaitOpen()
        {
            var inscriptionsContainer = new Dictionary<string, string>();

            foreach (var localizedString in _localizedStrings)
            {
                yield return localizedString.Value.AwaitLoad(text => 
                    inscriptionsContainer.Add(localizedString.Key, text));
            }
            
            _diContainer.BindFactory<ShopPresenter_Villager, ShopPresenter_Villager.Factory>()
                .WithArguments(_shopViewPrefab, _prefab, _initData, inscriptionsContainer, _studioEvent, _background);

            var factory = _diContainer.TryResolve<ShopPresenter_Villager.Factory>();
            factory.Create();
            _diContainer.Unbind<ShopPresenter_Villager.Factory>();
        }
    }
}