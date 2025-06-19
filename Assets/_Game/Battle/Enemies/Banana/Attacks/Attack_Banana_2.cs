using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Attack_Banana_2 : Attack
    {
        const float TIME_BETWEEN_ATTACKS = 1.5f;
        
        [SerializeField]
        private Shell_Banana _shellBananaPrefab;
   
        [SerializeField]
        private Transform[] _spawnPoints;
        
        private Coroutine _coroutine;
        private readonly List<IShell> _shells = new();

        public override Heart.Mode GetStartHeartMode => Heart.Mode.Red;
        public override int GetTurnAddedProgress => 2;
        public override Vector2 GetSizeArena => new(3f, 3f);
        public override int GetShieldAddedProgress => 2;

        private void Start()
        {
            _coroutine = StartCoroutine(WaitAttack());
        }

        public override void Hide()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);

            foreach (var shell in _shells) 
                shell?.Hide();
        }

        private IEnumerator WaitAttack()
        {
            while (true)
            {
                var points = _spawnPoints.ToList();
                
                var random = Random.Range(0, _spawnPoints.Length);
                points.Remove(_spawnPoints[random]);

                foreach (var spawnPoint in points) 
                    CreateShell(spawnPoint.position, -1);

                yield return new WaitForSeconds(TIME_BETWEEN_ATTACKS);
            }
        }
        
        private void CreateShell(Vector2 spawnPoint, float directionX = 1)
        {
            var shell = Instantiate(_shellBananaPrefab, spawnPoint, Quaternion.identity, transform);
            shell.SetDirection(new Vector2(directionX, 0));
            _shells.Add(shell);
        }
    }
}