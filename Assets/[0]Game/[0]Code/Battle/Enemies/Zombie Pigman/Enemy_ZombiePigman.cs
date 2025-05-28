using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Game
{
    // Класс персонажа "Свинозомби"
    public class Enemy_ZombiePigman : EnemyBase
    {
        [SerializeField]
        private SpriteRenderer _sword;

        [SerializeField]
        private GameObject _warning;

        [SerializeField]
        private ItemConfig _goldSword;
        
        [Inject]
        private MainInventory _mainInventory;
        
        [Inject]
        private WalletService _walletService;
        
        private bool _isSelectedInfo;
        private bool _isBuyerSword;

        public bool IsBuySword => _isBuyerSword;

        public override ActionBattle[] Actions
        {
            get
            {
                var actions = new List<ActionBattle>();

                for (var i = 0; i < _actions.Length; i++)
                {
                    if (i == 1 && !_isSelectedInfo)
                        continue;

                    if (i == 0 && _isSelectedInfo)
                        continue;
                    
                    if (i == 2 && !_mainInventory.IsGetItem("Carrot"))
                        continue;
                    
                    if (i == 3 && (_isBuyerSword || _walletService.Money.Value < 15))
                        continue;
                    
                    var action = _actions[i];
                    actions.Add(action);
                }

                return actions.ToArray();
            }
        }

        public override void Death(int damage)
        {
            _isStartDeathAnimation = true;
            _sword.gameObject.SetActive(false);
            _animator.SetTrigger(DeathHash);

            _shakeSequence.Kill();
            _deathSequence.Kill();
            _deathSequence = DOTween.Sequence();
            _deathSequence
                .AppendInterval(0.5f)
                .OnComplete(() => _deathAnimation.StartAnimation());
        }

        public void SetWarning(bool value) => 
            _warning.SetActive(value);

        public override void EndEnemyTurn(int turn)
        {
            if (turn > 4)
            {
                Mercy = 100;
            }
        }

        public override string GetReaction(BattleActionType actionType, Item item = null)
        {
            switch (actionType)
            {
                case BattleActionType.Attack:
                    if (Health < MaxHealth * 0.3f) 
                        return "Можно не надо?";
                    else
                        return "Эээ...";

                case BattleActionType.Mercy:
                    if (_isBuyerSword)
                    {
                        return "С тобой приятно иметь дело, давай пока";
                    }
                    
                    return "Я обьелся мне лень драться";
                    // return "Он был моим лучшим другом, ну лан пофиг";

                case BattleActionType.NoAction:
                    if (_isBuyerSword)
                    {
                        return "А чем сражаться?";
                    }
                    
                    return "Хрю-хрю";
                
                default:
                    return "(...)";
            }
        }

        public override string GetDeathReaction()
        {
            return "О нет я умер";
        }

        public override string GetStartReaction(int index)
        {
            return "Я низачто не проиграю";
        }

        public override string GetActionReaction(ActionBattle actionBattle)
        {
            var action = Actions.First(x => x.Name == actionBattle.Name);
            
            if (actionBattle.Name == _actions[2].Name)
            {
                _mainInventory.TryRemoveItem("Carrot");
                _isSelectedInfo = true;
            }

            if (actionBattle.Name == _actions[3].Name)
            {
                _isBuyerSword = true;
                _sword.gameObject.SetActive(false);
                _walletService.TrySellMoney(15);
                _mainInventory.Add(_goldSword.Prototype.Clone());
            }
            
            return action.Reaction;
        }
    }
}