using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game
{
    public sealed class Attack_WitherSkeleton_Tutorial : Attack
    {
        private const float TIME_DELAY_SPAWN_ATTACK = 1;
        private const float TIME_BETWEEN_ATTACKS = 4f;

        [SerializeField]
        private Shell_Banana _shellBananaPrefab;

        [SerializeField]
        private Transform[] _spawnPoints;

        [SerializeField]
        private GameObject _arrow;

        [SerializeField]
        private TriggerHandler _triggerHandler;
        
        [SerializeField]
        private GameObject _platform;

        private Coroutine _coroutine;
        private readonly List<IShell> _shells = new();
        public override Heart.Mode GetStartHeartMode => Heart.Mode.Blue;
        public override int GetTurnAddedProgress => 0;
        private bool _isJumped;


        [Inject]
        private TurnProgressStorage _turnProgressStorage;

        public override Vector2 GetSizeArena => new(2f, 2f);
        public override int GetShieldAddedProgress => 1;


        private void Start()
        {
            _coroutine = StartCoroutine(WaitAttack());
            _triggerHandler.TriggerEnter += OnHandlerTriggerEnter;
        }

        private void OnHandlerTriggerEnter()
        {
            _triggerHandler.TriggerEnter -= OnHandlerTriggerEnter;
            _isJumped = true;
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
            _arrow.SetActive(false);
            _platform.SetActive(false);

            yield return new WaitForSeconds(1);
            _arrow.SetActive(true);
            _platform.SetActive(true);

            _isJumped = false;
            yield return new WaitUntil(() => _isJumped);
            _arrow.SetActive(false);
            
            var random = Random.Range(0, _spawnPoints.Length);
            var spawnPoint = _spawnPoints[random];
            var directionX = random == 1 ? 1 : -1;
            
            yield return new WaitForSeconds(TIME_DELAY_SPAWN_ATTACK); 
            CreateShell(spawnPoint.position, directionX);
            yield return new WaitForSeconds(TIME_BETWEEN_ATTACKS);
            
            _turnProgressStorage.AddBattleProgress(100);
        }

        private void CreateShell(Vector2 spawnPoint, float directionX = 1)
        {
            var shell = Instantiate(_shellBananaPrefab, spawnPoint, Quaternion.identity, transform);
            shell.SetDirection(new Vector2(directionX, 0));
            _shells.Add(shell);
        }
    }
}