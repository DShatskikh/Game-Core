using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Attack_WitherSkeleton_1 : Attack
    {
        private const float TIME_BETWEEN_ATTACKS = 1f;

        [SerializeField]
        private Shell_Banana _shellBananaPrefab;

        [SerializeField]
        private Transform _spawnPoint;

        private Coroutine _coroutine;
        private readonly List<IShell> _shells = new();
        public override Heart.Mode GetStartHeartMode => Heart.Mode.Blue;
        public override int GetTurnAddedProgress => 2;

        public override Vector2 GetSizeArena => new(4f, 2f);
        public override int GetShieldAddedProgress => 3;

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
                CreateShell(_spawnPoint.position, -1);
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