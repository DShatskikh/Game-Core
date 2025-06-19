using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    // Моб арена
    public sealed class MobArena : MonoBehaviour, IGameBattleListener
    {
        [Serializable]
        public struct Data
        {
            public int BattleIndex; 
        }
        
        [SerializeField]
        private StarterBattleBase[] _battles;

        [SerializeField]
        private GameObject _timer;
        
        [SerializeField]
        private TMP_Text _timerLabel;

        private int _battleIndex;
        private IGameRepository _gameRepository;

        [Inject]
        private void Construct(IGameRepositoryStorage gameRepository)
        {
            _gameRepository = gameRepository;
        }
        
        private void Start()
        {
            if (_gameRepository.TryGet(SaveConstants.MOB_ARENA, out Data data))
            {
                _battleIndex = data.BattleIndex;
            }
            
            _battles[_battleIndex].gameObject.SetActive(true);
        }

        public void HideAllMons()
        {
            enabled = false;
            
            foreach (var battle in _battles)
            {
                battle.gameObject.SetActive(false);
            }
        }
        
        public void OnOpenBattle() { }

        public void OnCloseBattle()
        {
            if (!this)
                return;
         
            if (!enabled)
                return;
            
            _battleIndex++;
            _gameRepository.Set(SaveConstants.MOB_ARENA, new Data()
            {
                BattleIndex = _battleIndex
            });
            
            if (_battles.Length == _battleIndex)
                return;
            
            StartCoroutine(AwaitTimer());
        }

        private IEnumerator AwaitTimer()
        {
            _timer.SetActive(true);

            var timer = 5;

            while (timer > 0)
            {
                _timerLabel.text = $"Следующий противник появится через {timer}";
                timer--;
                yield return new WaitForSeconds(1);
            }
            
            _timer.SetActive(false);
            _battles[_battleIndex].gameObject.SetActive(true);
        }
    }
}