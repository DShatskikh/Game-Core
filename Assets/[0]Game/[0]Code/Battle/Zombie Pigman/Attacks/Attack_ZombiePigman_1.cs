using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Attack_ZombiePigman_1 : Attack
    {
        [SerializeField]
        private Shell_Sword_ZombiePigman _attackPrefab;
        
        [SerializeField]
        private Transform[] _spawnPoints;
        
        private Coroutine _coroutine;
        private HeartModeService _heartModeService;
        private Arena _arena;
        private float _direction = 1;
        private List<Shell> _shells = new();

        [Inject]
        private void Construct(HeartModeService heartModeService, Arena arena)
        {
            _heartModeService = heartModeService;
            _arena = arena;
        }
        
        private void Start()
        {
            _direction = Random.Range(0, 2) == 0 ? -1 : 1;
            _coroutine = StartCoroutine(WaitAttack());
        }

        private IEnumerator WaitAttack()
        {
            _heartModeService.SetMode(Heart.Mode.Blue);
            yield return _arena.AwaitSetSize(new Vector2(5, 1.5f));
            
            while (true)
            {
                var spawnPoint = _direction == 1 ? _spawnPoints[0] : _spawnPoints[1];
                var shell = Instantiate(_attackPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
                shell.SetDirection(_direction);
                _shells.Add(shell);
                yield return new WaitForSeconds(1.5f);
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