using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public sealed class Attack_Dimas_1 : Attack
    {
        private const float TIME_BETWEEN_ATTACKS = 1.5f;

        [SerializeField]
        private Shell_Note _shellPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        private Coroutine _coroutine;
        private readonly List<IShell> _shells = new();
        private Coroutine _spawnAttackProcess;
        private bool _isUp;
        public override Heart.Mode GetStartHeartMode => Heart.Mode.Red;
        public override int GetTurnAddedProgress => 1;

        public override Vector2 GetSizeArena => new(4f, 4f);
        public override int GetShieldAddedProgress => 4;

        private void Start()
        {
            _coroutine = StartCoroutine(WaitAttack());
        }

        public override void Hide()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            if (_spawnAttackProcess != null)
                StopCoroutine(_spawnAttackProcess);
            
            foreach (var shell in _shells) 
                shell?.Hide();
        }

        private IEnumerator WaitSpawnAttack()
        {
            var addedY = _isUp ? Random.Range(0.5f, 1.5f) : Random.Range(-1.5f, -0.5f);
            _isUp = !_isUp;
                
            for (int i = 0; i < 5; i++)
            {
                CreateShell(_spawnPoint.position.AddY(addedY)); 
                yield return new WaitForSeconds(0.5f); 
            }
        }
        
        private IEnumerator WaitAttack()
        {
            while (true)
            {
                _spawnAttackProcess = StartCoroutine(WaitSpawnAttack());
                yield return new WaitForSeconds(TIME_BETWEEN_ATTACKS); 
            }
        }

        private void CreateShell(Vector2 spawnPoint)
        {
            var shell = Instantiate(_shellPrefab, spawnPoint, Quaternion.identity, transform);
            _shells.Add(shell);
        }
    }
}