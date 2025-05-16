// using System.Collections;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
// using I2.Loc;
// using Zenject;
//
// namespace Game
// {
//     public class StartBattle_Herobrine : MonoBehaviour
//     {
//         [SerializeField]
//         private BattleInstaller _installerPrefab;
//
//         [SerializeField]
//         private BattleController_Herobrine.InitData _initData;
//         
//         [SerializeField]
//         private AudioClip _music;
//         
//         [SerializeField]
//         private SerializableDictionary<string, LocalizedString> _localizedStrings;
//         
//         private DiContainer _diContainer;
//
//         [Inject]
//         private void Construct(DiContainer diContainer)
//         {
//             _diContainer = diContainer;
//         }
//         
//         private void Start()
//         {
//             Open();
//         }
//
//         [Button]
//         private void Open()
//         {
//             gameObject.SetActive(true);
//             var inscriptionsContainer = new Dictionary<string, string>();
//
//             foreach (var localizedString in _localizedStrings)
//             {
//                 inscriptionsContainer.Add(localizedString.Key, localizedString.Value);
//             }
//             
//             var installer = Instantiate(_installerPrefab);
//             _diContainer.Inject(installer);
//             
//             installer.CreatePresenterCommand = () =>
//             {
//                 _diContainer.BindFactory<BattleController_Herobrine, BattleController_Herobrine.Factory>()
//                     .WithArguments(_initData, _music, inscriptionsContainer);
//                 
//                 var factory = _diContainer.TryResolve<BattleController_Herobrine.Factory>();
//                 _diContainer.Unbind<BattleController_Herobrine.Factory>();
//                 return factory.Create();
//             };
//             
//             installer.InstallBindings();
//         }
//     }
// }