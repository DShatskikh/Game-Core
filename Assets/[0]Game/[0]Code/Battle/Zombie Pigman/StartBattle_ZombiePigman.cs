using FMODUnity;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class StartBattle_ZombiePigman : MonoBehaviour
    {
        [SerializeField]
        private BattleInstaller _installerPrefab;

        [SerializeField]
        private BattleController_ZombiePigman.InitData _initData;
        
        [SerializeField]
        private StudioEventEmitter _studioEventEmitter;
        
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
            
            var installer = Instantiate(_installerPrefab, transform.position.SetZ(0), Quaternion.identity);
            _diContainer.Inject(installer);
            
            installer.CreatePresenterCommand = () =>
            {
                _diContainer.BindFactory<BattleController_ZombiePigman, BattleController_ZombiePigman.Factory>()
                    .WithArguments(_initData, _studioEventEmitter, _localizedStrings);
                
                var factory = _diContainer.TryResolve<BattleController_ZombiePigman.Factory>();
                _diContainer.Unbind<BattleController_ZombiePigman.Factory>();
                return factory.Create();
            };
            
            installer.InstallBindings();
        }
    }
}