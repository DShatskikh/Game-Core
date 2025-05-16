using System.Collections;
using TMPro;
using UnityEngine;

namespace Game
{
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
            _battles[0].gameObject.SetActive(true);
        }

        public void OnOpenBattle()
        {
            
        }

        public void OnCloseBattle()
        {
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
                _timerLabel.text = timer.ToString();
                timer--;
                yield return new WaitForSeconds(1);
            }
            
            _timer.SetActive(false);
            _battles[_battleIndex].gameObject.SetActive(true);
        }
    }
}