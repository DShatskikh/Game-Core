﻿using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Game
{
    public class StartBattle_Zombie : MonoBehaviour
    {
        [SerializeField]
        private BattleInstaller _installerPrefab;

        [SerializeField]
        private BattleController_Zombie.InitData _initData;
        
        [SerializeField]
        private AudioClip _music;
        
        [SerializeField]
        private SerializableDictionary<string, LocalizedString> _localizedStrings;

        [SerializeField]
        private DialogueSystemTrigger _winDialog;

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
            
            var installer = Instantiate(_installerPrefab, Camera.main.transform.position.SetZ(0), Quaternion.identity);
            _diContainer.Inject(installer);
            
            installer.CreatePresenterCommand = () =>
            {
                _diContainer.BindFactory<BattleController_Zombie, BattleController_Zombie.Factory>()
                    .WithArguments(_initData, _music, inscriptionsContainer, _winDialog);
                
                var factory = _diContainer.TryResolve<BattleController_Zombie.Factory>();
                _diContainer.Unbind<BattleController_Zombie.Factory>();
                return factory.Create();
            };
            
            installer.InstallBindings();
        }
    }
}