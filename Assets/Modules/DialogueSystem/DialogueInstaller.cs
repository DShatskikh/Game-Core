﻿using UnityEngine;
using Zenject;

namespace Game
{
    // Инсталлер для диалогового окна
    public class DialogueInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();

            foreach (var monoBehaviour in GetComponentsInChildren<MonoBehaviour>())
            {
                Container.Inject(gameObject);
                Container.Inject(monoBehaviour);
            }
        }
    }
}