using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game
{
    public class StartBattle_Herobrine : MonoBehaviour
    {
        [SerializeField]
        private BattleInstaller _installerPrefab;

        [SerializeField]
        private BattlePresenter_Herobrine.InitData _initData;
        
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
            var installer = Instantiate(_installerPrefab);
            _diContainer.Inject(installer);
            
            installer.CreatePresenterCommand = () =>
            {
                _diContainer.BindFactory<BattlePresenter_Herobrine, BattlePresenter_Herobrine.Factory>()
                    .WithArguments(_initData);
                
                var factory = _diContainer.TryResolve<BattlePresenter_Herobrine.Factory>();
                _diContainer.Unbind<BattlePresenter_Herobrine.Factory>();
                return factory.Create();
            };
            
            installer.InstallBindings();
            
            yield break;
        }
    }
}