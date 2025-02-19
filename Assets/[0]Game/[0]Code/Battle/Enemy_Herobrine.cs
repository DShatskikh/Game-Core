using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class Enemy_Herobrine : MonoBehaviour, IEnemy
    {
        [SerializeField]
        private BattleMessageBox _messageBox;

        [SerializeField]
        private Attack[] _attacks;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        
        private int _health;

        public Attack[] GetAttacks => _attacks;
        public BattleMessageBox GetMessageBox => _messageBox;
        public int GetHealth => _health;

        private void Start()
        {
            _health = _maxHealth;
        }

        public void Damage(int damage)
        {
            _health -= damage;
            StartCoroutine(WaitDamageAnimation());
        }

        private IEnumerator WaitDamageAnimation()
        {
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            _spriteRenderer.color = Color.white;
        }
    }
}