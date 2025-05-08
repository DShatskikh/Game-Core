using System.Linq;
using DG.Tweening;
using I2.Loc;
using UnityEngine;

namespace Game
{
    public class Enemy_ZombiePigman : MonoBehaviour, IEnemy
    {
        private static readonly int DamageHash = Animator.StringToHash("Damage");
        private static readonly int DeathHash = Animator.StringToHash("Death");

        [SerializeField]
        private LocalizedString _name;
        
        [SerializeField]
        private BattleMessageBox _messageBox;

        [SerializeField]
        private Attack[] _attacks;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private TextMesh _damageLabelPrefab;

        [SerializeField]
        private ActionBattle[] _actions;

        [SerializeField]
        private DeathAnimation _deathAnimation;
        
        [SerializeField]
        private int _op;
        
        [SerializeField]
        private int _money;
        
        private int _health;
        private Sequence _damageSequence;
        private Sequence _deathSequence;
        private Sequence _shakeSequence;
        private bool _isStartDeathAnimation;

        public ActionBattle[] Actions => _actions;
        public LocalizedString Name => _name;
        public Attack[] Attacks => _attacks;
        public BattleMessageBox MessageBox => _messageBox;
        public int Health => _health;
        public int MaxHealth => _maxHealth;
        public int Mercy { get; set; }
        public bool CanMercy => Mercy == 100 || Health <= 6;
        public bool IsMercy { get; set; }
        public int GetOP => _op;
        public int GetMoney => _money;

        private void Start()
        {
            _health = _maxHealth;
        }

        public void Damage(int damage)
        {
            if (CanMercy)
            {
                damage = _health;
            }
            
            _health -= damage;
            _animator.SetTrigger(DamageHash);

            var damageLabel = Instantiate(_damageLabelPrefab, transform.position.AddY(0.5f), Quaternion.identity);
            damageLabel.color = Color.red;
            damageLabel.text = $"-{damage}";

            _damageSequence?.Kill();
            _damageSequence = DOTween.Sequence();
            _damageSequence
                .Append(damageLabel.transform.DOJump(transform.position.AddX(1), 1, 1, 1))
                .Append(damageLabel.transform.DOMoveY(transform.position.AddY(1).y, 0.5f)).OnStepComplete(
                    () =>
                    {
                        if (Health <= 0 && !_isStartDeathAnimation)
                        {
                            _shakeSequence.Kill();
                            _shakeSequence = DOTween.Sequence();
                            _shakeSequence
                                .Append(transform.DOShakePosition(1, 0.05f, 100))
                                .SetLoops(-1, LoopType.Restart);
                        }
                    })
                .Insert(1f, damageLabel.transform.DOScaleY(2, 0.5f))
                .Insert(1f, DOTween.To(x => damageLabel.color = damageLabel.color.SetA(x), 1, 0, 0.5f))
                .OnComplete(() =>
                    {
                        Destroy(damageLabel.gameObject);
                    });
        }

        public void Death(int damage)
        {
            _isStartDeathAnimation = true;
            _animator.SetTrigger(DeathHash);
            
            _shakeSequence.Kill();
            _deathSequence.Kill();
            _deathSequence = DOTween.Sequence();
            _deathSequence
                .AppendInterval(0.5f)
                .OnComplete(() => _deathAnimation.StartAnimation());
        }

        public void Spared()
        {
            gameObject.SetActive(false);
        }

        public string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    if (Health < MaxHealth * 0.3f) 
                        return "Я передумал проверять документы";
                    else
                        return "Это не потоварищески";

                case BattleActionType.Item:
                    if (item != null && item.MetaData.Name == "Лечебная трава")
                        return "(Зомби смотрит с интересом)";
                    else
                        return "(Зырк)";

                case BattleActionType.Mercy:
                    return "(Всё впорядке вы гражданан сервера)";

                case BattleActionType.NoAction:
                    return "Покажите свои документы";
                
                default:
                    return "(...)";
            }
        }

        public string GetDeathReaction()
        {
            return "О нет я умер";
        }

        public string GetDeathFriendReaction(IEnemy enemy)
        {
            return "О нет! он был моим лучшим другом!";
        }
        
        public string GetStartReaction(int index)
        {
            return "Здравствуйте товарищ, покажите ваши документы";
        }

        public string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            return action.Reaction;
        }

        public string GetReaction(ActionBattle actionBattle)
        {
            return actionBattle.Reaction;
        }
    }
}