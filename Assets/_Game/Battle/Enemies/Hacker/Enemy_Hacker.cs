using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Game
{
    // Реализация противника Хакер
    public sealed class Enemy_Hacker : EnemyBase
    {
        public override bool IsDeath => false;

        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    var action = _actions[i];
                    actions.Add(action);
                }

                return actions.ToArray();
            }
        }

        public override void Damage(int damage)
        {
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

        public override string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    return "ПХПХПХ. Ты даже меня не поцарапал";

                case BattleActionType.Mercy:
                    return "...";

                case BattleActionType.NoAction:
                    return "Узри же мое величие и силу читов!";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "...";
        }

        public override string GetStartReaction(int index)
        {
            return "Я ВЕЛИЧАЙШИЙ ХАКЕР ВСЕХ ВРЕМЕН!";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            return action.Reaction;
        }
    }
}