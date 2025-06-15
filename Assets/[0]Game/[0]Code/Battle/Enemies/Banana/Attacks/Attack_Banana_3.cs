using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Attack_Banana_3 : Attack
    {
        const float TIME_BETWEEN_ATTACKS = 0.5f;
        
        [SerializeField]
        private Shell_Banana _shellBananaPrefab;
        
        [SerializeField]
        private Shell_BananaHealth _shellBananaHealthPrefab;
        
        [SerializeField]
        private Transform[] _spawnPoints;
        
        private Coroutine _coroutine;
        private HeartModeService _heartModeService;
        private TimeBasedTurnBooster _timeBasedTurnBooster;
        private readonly List<IShell> _shells = new();

        public override Vector2 GetSizeArena => new(3f, 3f);
        public override int GetAddProgress => 1;

        [Inject]
        private void Construct(HeartModeService heartModeService, TimeBasedTurnBooster timeBasedTurnBooster)
        {
            _heartModeService = heartModeService;
            _timeBasedTurnBooster = timeBasedTurnBooster;
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
            _timeBasedTurnBooster.SetAddedProgress(0);
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
            if (Random.Range(0, 10) == 3)
            {
                var shellHealth = Instantiate(_shellBananaHealthPrefab, spawnPoint, Quaternion.identity, transform);
                shellHealth.SetDirection(new Vector2(directionX, 0));
                _shells.Add(shellHealth);
                return;
            }
            
            var shell = Instantiate(_shellBananaPrefab, spawnPoint, Quaternion.identity, transform);
            shell.SetDirection(new Vector2(directionX, 0));
            _shells.Add(shell);
        }
    }
}