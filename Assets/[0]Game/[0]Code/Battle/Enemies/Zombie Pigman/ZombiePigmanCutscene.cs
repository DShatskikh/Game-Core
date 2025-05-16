using System.Collections;
using DG.Tweening;
using FMODUnity;
using PixelCrushers.DialogueSystem.Wrappers;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class ZombiePigmanCutscene : MonoBehaviour, IGameCutsceneListener
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        [SerializeField]
        private StarterBattleBase _startBattle;

        [SerializeField]
        private Enemy_ZombiePigman[] _otherPigmans;
        
        private Enemy_ZombiePigman _enemyZombiePigman;
        private Player _player;
        private bool _isOpenDialogue;
        private MainInventory _mainInventory;

        [Inject]
        private void Construct(Player player, MainInventory mainInventory)
        {
            _player = player;
            _enemyZombiePigman = GetComponent<Enemy_ZombiePigman>();
            _mainInventory = mainInventory;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                Destroy(GetComponent<BehaviorGraphAgent>());
                GetComponent<EnemyMover>().StopMove();
                _dialogueSystemTrigger.OnUse();
                _isOpenDialogue = true;
            }
        }

        public void OnShowCutscene()
        {
            
        }

        public void OnHideCutscene()
        {
            if (!_isOpenDialogue)
                return;

            _isOpenDialogue = false;
            StartCoroutine(AwaitCutscene());
        }

        private IEnumerator AwaitCutscene()
        {
            _player.PlaySwordAttack();
            yield return new WaitForSeconds(0.5f);
            _enemyZombiePigman.Damage(1);
            _mainInventory.MainSlots[0].Item.TryGetComponent(out AttackComponent attackComponent);
            Instantiate(attackComponent.Effect, _enemyZombiePigman.transform.position.AddY(0.5f), Quaternion.identity);
            yield return _enemyZombiePigman.transform.DOJump(_enemyZombiePigman.transform.position.AddX(1), 1, 1, 1);
            yield return new WaitForSeconds(1);

            if (_otherPigmans.Length != 0)
            {
                foreach (var pigman in _otherPigmans)
                {
                    pigman.SetWarning(true);
                    Destroy(pigman.GetComponent<BehaviorGraphAgent>());
                    pigman.GetComponent<EnemyMover>().StopMove();
                }
                
                RuntimeManager.PlayOneShot("event:/Звуки/Битва/Начало боя");
                yield return new WaitForSeconds(1);
                
                foreach (var pigman in _otherPigmans)
                {
                    pigman.SetWarning(false);
                }
            }
            
            _startBattle.StartBattle();
        }
    }
}