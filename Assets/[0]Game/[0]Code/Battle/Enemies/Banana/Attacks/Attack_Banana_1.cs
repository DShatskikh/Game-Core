using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Attack_Banana_1 : Attack
    {
        const float TIME_BETWEEN_ATTACKS = 1.5f;
        
        [SerializeField]
        private Shell_Banana _shellBananaPrefab;
   
        [SerializeField]
        private Transform[] _spawnPoints;
        
        private Coroutine _coroutine;
        private HeartModeService _heartModeService;
        private readonly List<IShell> _shells = new();
        
        public override Vector2 GetSizeArena => new(3f, 3f);
        public override int GetAddProgress => 1;

        [Inject]
        private void Construct(HeartModeService heartModeService)
        {
            _heartModeService = heartModeService;
        }
        
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
            _heartModeService.SetMode(Heart.Mode.Red);
            
            while (true)
            {
                var random = Random.Range(0, _spawnPoints.Length);
                var spawnPoint = _spawnPoints[random];
                var directionX = random == 1 ? 1 : -1;
                    
                CreateShell(spawnPoint.position, directionX);
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