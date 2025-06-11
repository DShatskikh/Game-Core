using System.Collections;
using DG.Tweening;
using FMODUnity;
using PixelCrushers.DialogueSystem;
using Unity.Behavior;
using UnityEngine;
using Zenject;
using DialogueSystemTrigger = PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger;

namespace Game
{
    // Катсцена перед битвой с персонажем "Свинозомби"
    public sealed class ZombiePigmanCutscene : MonoBehaviour
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        [SerializeField]
        private StarterBattleBase _startBattle;

        [SerializeField]
        private Enemy_ZombiePigman[] _otherPigmans;

        [SerializeField]
        private GameObject _tutorialArrow;
        
        private Enemy_ZombiePigman _enemyZombiePigman;
        private Player _player;
        private MainInventory _mainInventory;
        private EnemyMover _mover;

        [Inject]
        private void Construct(Player player, MainInventory mainInventory)
        {
            _player = player;
            _mainInventory = mainInventory;
            _enemyZombiePigman = GetComponent<Enemy_ZombiePigman>();
            _mover = GetComponent<EnemyMover>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Player>())
            {
                Destroy(GetComponent<BehaviorGraphAgent>());
                _dialogueSystemTrigger.OnUse();
                
                if (_tutorialArrow)
                    _tutorialArrow.SetActive(false);
                
                _mover.StopMove();
                
                StartCoroutine(AwaitCutscene());
            }
        }

        private IEnumerator AwaitCutscene()
        {
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _player.PlaySwordAttack();
            yield return new WaitForSeconds(0.5f);
            _enemyZombiePigman.Damage(1);
            _mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent);
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