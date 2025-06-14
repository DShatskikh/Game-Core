using System.Collections;
using TMPro;
using UnityEngine;

namespace Game
{
    // Моб арена
    public sealed class MobArena : MonoBehaviour, IGameBattleListener
    {
        [SerializeField]
        private StarterBattleBase[] _battles;

        [SerializeField]
        private GameObject _timer;
        
        [SerializeField]
        private TMP_Text _timerLabel;

        private int _battleIndex;
        
        private void Start()
        {
            _battles[_battleIndex].gameObject.SetActive(true);
        }

        public void OnOpenBattle()
        {
            
        }

        public void OnCloseBattle()
        {
            if (!this)
                return;
            
            if (!gameObject)
                return;
            
            if (!gameObject.activeSelf)
                return;
            
            _battleIndex++;
            
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