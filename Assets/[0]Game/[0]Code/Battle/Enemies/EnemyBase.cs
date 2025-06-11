using DG.Tweening;
using I2.Loc;
using Unity.Behavior;
using UnityEngine;
using Zenject;

namespace Game
{
    // Базовый класс противника
    public abstract class EnemyBase : MonoBehaviour, IEnemy, IGameBattleListener
    {
        private const string IDLE_HASH = "Idle";
        private protected static readonly int DamageHash = Animator.StringToHash("Damage");
        private protected  static readonly int DeathHash = Animator.StringToHash("Death");

        [SerializeField]
        private string _id;
        
        [SerializeField]
        private LocalizedString _name;

        [SerializeField]
        private int _maxHealth;

        [SerializeField]
        private int _op;

        [SerializeField]
        private int _money;

        [SerializeField]
        private Attack[] _attacks;

        [SerializeField]
        private protected ActionBattle[] _actions;

        [SerializeField]
        private protected TextMesh _damageLabelPrefab;

        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        private protected int _health;
        private BattleMessageBox _messageBox;
        private protected Animator _animator;
        private protected DeathAnimation _deathAnimation;
        private protected Sequence _damageSequence;
        private GameStateController _gameStateController;
        private protected Sequence _deathSequence;
        private protected Sequence _shakeSequence;
        private protected bool _isStartDeathAnimation;
        private int _mercy;

        public LocalizedString Name => _name;
        public Attack[] Attacks => _attacks;
        public BattleMessageBox MessageBox => _messageBox;
        public abstract ActionBattle[] Actions { get; }
        public int Health => _health;
        public virtual bool IsDeath => _health <= 0;
        public int MaxHealth => _maxHealth;

        public int Mercy
        {
            get => _mercy;
            set
            {
                if (value > 100)
                {
                    _mercy = 100;
                    return;
                }

                _mercy = value;
            }
        }

        public bool CanMercy => Mercy == 100 || Health <= 6;
        public bool IsMercy { get; set; }
        public int GetOP => _op;
        public int GetMoney => _money;
        public string GetID => _id;

        // Получаем ссылки нужные для работы класса
        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
            
            _messageBox = GetComponentInChildren<BattleMessageBox>(true);
            _animator = GetComponent<Animator>();
            _deathAnimation = GetComponent<DeathAnimation>();
        }
        
        private void Start()
        {
            _health = _maxHealth;
        }

        private void OnDestroy()
        {
            _gameStateController.RemoveListener(this);
        }
        
        // Событие получения урона
        public virtual void Damage(int damage)
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
                        if (IsDeath && !_isStartDeathAnimation)
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

        // Событие смерти

        public virtual void Death(int damage)
        {
            _isStartDeathAnimation = true;
            _animator.SetTrigger(DeathHash);

            _shakeSequence.Kill();
            _deathSequence.Kill();
            _deathSequence = DOTween.Sequence();
            _deathSequence
                .OnComplete(() => _deathAnimation.StartAnimation());
        }

        // Событие пощады

        public virtual void Spared()
        {
            gameObject.SetActive(false);
        }

        // Событие начала битвы

        public void OnOpenBattle()
        {
            if (this == null)
                return;
            
            var startBattle = GetComponentInParent<StarterBattleBase>(true);
            
            if (!startBattle || !startBattle.gameObject.activeSelf)
                return;
            
            if (GetComponent<BehaviorGraphAgent>())
                Destroy(GetComponent<BehaviorGraphAgent>());
            
            _animator.CrossFade(IDLE_HASH, 0);
            _spriteRenderer.transform.localScale = _spriteRenderer.transform.localScale.SetX(1);
        }


        public void OnCloseBattle() { }

        // Реакция противника на действие
        public abstract string GetReaction(BattleActionType actionType, Item item = null);
        public abstract string GetDeathReaction();
        public abstract string GetStartReaction(int index);
        public abstract string GetActionReaction(ActionBattle actionBattle);
        public virtual string GetDeathFriendReaction(IEnemy enemy) => "...";
        public virtual void EndEnemyTurn(int turn) { }
        private protected virtual void OnDamage() { }
    }
}