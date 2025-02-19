using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class StartBattle_Herobrine : MonoBehaviour
    {
        [SerializeField]
        private BattleInstaller _installerPrefab;

        [SerializeField]
        private BattlePresenterBaseHerobrine.InitData _initData;
        
        [SerializeField]
        private AudioClip _music;
        
        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;
        
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
        private void Open()
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
            
            var installer = Instantiate(_installerPrefab);
            _diContainer.Inject(installer);
            
            installer.CreatePresenterCommand = () =>
            {
                _diContainer.BindFactory<BattlePresenterBaseHerobrine, BattlePresenterBaseHerobrine.Factory>()
                    .WithArguments(_initData, _music, inscriptionsContainer);
                
                var factory = _diContainer.TryResolve<BattlePresenterBaseHerobrine.Factory>();
                _diContainer.Unbind<BattlePresenterBaseHerobrine.Factory>();
                return factory.Create();
            };
            
            installer.InstallBindings();
        }
    }
}