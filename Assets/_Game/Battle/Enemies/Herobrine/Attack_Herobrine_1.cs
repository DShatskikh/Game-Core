﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Attack_Herobrine_1 : Attack
    {
        [SerializeField]
        private Shell _prefab;

        private List<IShell> _shells = new();
        private DiContainer _container;
        private Coroutine _coroutine;
        public override Heart.Mode GetStartHeartMode => Heart.Mode.Red;
        public override int GetTurnAddedProgress => 2;
        public override int GetShieldAddedProgress => 3;

        [Inject]
        private void Construct(DiContainer container, HeartModeService heartModeService)
        {
            _container = container;
        }
        
        private void Start()
        {
            _coroutine = StartCoroutine(WaitAttack());
        }

        private IEnumerator WaitAttack()
        {
            while (true)
            {
                var point = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f)).normalized * 3;
                
                var shell = Instantiate(_prefab, (Vector2)transform.position + point, Quaternion.identity, transform);
                _container.Inject(shell);
                _shells.Add(shell);
                yield return new WaitForSeconds(2);
            }
        }

        public override void Hide()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            
            foreach (var shell in _shells) 
                shell.Hide();
        }
    }
}