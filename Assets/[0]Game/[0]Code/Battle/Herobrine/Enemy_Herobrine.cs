// using System;
// using System.Collections;
// using I2.Loc;
// using UnityEngine;
//
// namespace Game
// {
//     public class Enemy_Herobrine : MonoBehaviour, IEnemy
//     {
//         [SerializeField]
//         private BattleMessageBox _messageBox;
//
//         [SerializeField]
//         private Attack[] _attacks;
//
//         [SerializeField]
//         private int _maxHealth;
//
//         [SerializeField]
//         private SpriteRenderer _spriteRenderer;
//         
//         private int _health;
//
//         public LocalizedString Name { get; }
//         public Attack[] Attacks => _attacks;
//         public BattleMessageBox MessageBox => _messageBox;
//         public int Health => _health;
//
//         private void Start()
//         {
//             _health = _maxHealth;
//         }
//
//         public void Damage(int damage)
//         {
//             _health -= damage;
//             StartCoroutine(WaitDamageAnimation());
//         }
//
//         public void Death(int damage)
//         {
//             throw new NotImplementedException();
//         }
//
//         private IEnumerator WaitDamageAnimation()
//         {
//             _spriteRenderer.color = Color.red;
//             yield return new WaitForSeconds(0.5f);
//             _spriteRenderer.color = Color.white;
//         }
//     }
// }