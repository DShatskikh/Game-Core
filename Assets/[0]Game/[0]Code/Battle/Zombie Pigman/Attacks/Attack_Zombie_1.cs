using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Attack_Zombie_1 : Attack
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
            yield return _arena.AwaitSetSize(new Vector2(5, 3));
            
            while (true)
            {
                var platformRight = Instantiate(_platformPrefab, _platformSpawnPoint[0].position, Quaternion.identity, _platformSpawnPoint[0]);
                platformRight.SetDirection(Vector2.left);
                yield return new WaitForSeconds(1f);
                
                var platformLeft = Instantiate(_platformPrefab, _platformSpawnPoint[1].position, Quaternion.identity, _platformSpawnPoint[1]);
                platformLeft.SetDirection(Vector2.right);
                yield return new WaitForSeconds(1f);
            }
        }
        
        public override void Hide()
        {
            
        }
    }
}