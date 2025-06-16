using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    // Атака 2 Свинозомби
    public sealed class Attack_ZombiePigman_2 : Attack
    {
        [SerializeField]
        private Platform _platformPrefab;

        [SerializeField]
        private Shell_Zombie_Jump _shellZombieJumpPrefab;
        
        [SerializeField]
        private Transform[] _platformSpawnPoint;
        
        private HeartModeService _heartModeService;
        private Coroutine _coroutine;
        private Arena _arena;
        private List<Platform> _platformsUp = new();
        private List<Platform> _platformsDown = new();
        private List<Shell> _shells = new();

        public override Vector2 GetSizeArena => new(5, 3);
        public override int GetAddProgress => 2;

        [Inject]
        private void Construct(HeartModeService heartModeService, Arena arena)
        {
            _heartModeService = heartModeService;
            _arena = arena;
        }
        
        private void Start()
        {
            _coroutine = StartCoroutine(WaitAttack());
        }

        private IEnumerator WaitAttack()
        {
            _heartModeService.SetMode(Heart.Mode.Blue);
            
            while (true)
            {
                var platformRight = Instantiate(_platformPrefab, _platformSpawnPoint[0].position, Quaternion.identity,
                    _platformSpawnPoint[0]);
                platformRight.SetDirection(Vector2.left);
                _platformsDown.Add(platformRight);
                yield return new WaitForSeconds(1f);
                
                var platformLeft = Instantiate(_platformPrefab, _platformSpawnPoint[1].position, Quaternion.identity,
                    _platformSpawnPoint[1]);
                platformLeft.SetDirection(Vector2.right);
                _platformsUp.Add(platformLeft);
                var shell = Instantiate(_shellZombieJumpPrefab, platformLeft.transform.position.AddY(0.075f), 
                    Quaternion.identity, platformLeft.transform);
                shell.Init(_platformsDown, transform);
                _shells.Add(shell);
                yield return new WaitForSeconds(1f);
            }
        }
        
        public override void Hide()
        {
            StopCoroutine(_coroutine);
            
            foreach (var shell in _shells)
            {
                shell.Hide();
            }

            foreach (var platform in _platformsDown)
            {
                platform.Hide();
            }
            
            foreach (var platform in _platformsUp)
            {
                platform.Hide();
            }
        }
    }
}