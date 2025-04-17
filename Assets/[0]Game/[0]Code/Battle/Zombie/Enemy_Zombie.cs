using DG.Tweening;
using I2.Loc;
using UnityEngine;

namespace Game
{
    public class Enemy_Zombie : MonoBehaviour, IEnemy
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
        
        private int _health;

        public ActionBattle[] Actions => _actions;
        public LocalizedString Name => _name;
        public Attack[] Attacks => _attacks;
        public BattleMessageBox MessageBox => _messageBox;
        public int Health => _health;
        public int MaxHealth => _maxHealth;
        public int Mercy  { get; set; }
        public bool IsMercy { get; set; }

        private void Start()
        {
            _health = _maxHealth;
        }

        public void Damage(int damage)
        {
            _health -= damage;
            _animator.SetTrigger(DamageHash);

            var damageLabel = Instantiate(_damageLabelPrefab, transform.position.AddY(0.5f), Quaternion.identity);
            damageLabel.color = Color.red;
            damageLabel.text = $"-{damage}";

            var sequence = DOTween.Sequence();
            sequence
                .Append(damageLabel.transform.DOJump(transform.position.AddX(1), 1, 1, 1))
                //.Append(damageLabel.transform.DOJump(transform.position.AddX(1), 0.25f, 1, 0.5f))
                .Append(damageLabel.transform.DOMoveY(transform.position.AddY(1).y, 0.5f))
                .Insert(1f, damageLabel.transform.DOScaleY(2, 0.5f))
                .Insert(1f, DOTween.To(x => damageLabel.color = damageLabel.color.SetA(x), 1, 0, 0.5f))
                .OnComplete(() => Destroy(damageLabel.gameObject));
        }

        public void Death(int damage)
        {
            _animator.SetTrigger(DeathHash);
        }
    }
}