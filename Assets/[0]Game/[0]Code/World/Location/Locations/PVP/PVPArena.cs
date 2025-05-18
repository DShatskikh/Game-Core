using System;
using System.Collections;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DialogueSystemTrigger = PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger;

namespace Game
{
    public sealed class PVPArena : MonoBehaviour
    {
        public enum State : byte
        {
            START = 0,
            BANANA = 1,
            DIMAS = 2,
            JULIANA = 3,
            TROLL = 4,
            FROST = 5,
            HACKER = 6,
            HEROBRINE = 7,
            END = 8
        }
        
        [Serializable]
        public struct Data
        {
            public State State;
        }

        [Serializable]
        public struct Enemy
        {
            public StarterBattleBase StartBattle;
            public GameObject GameObject;
            public DialogueSystemTrigger KillReplica;
            public DialogueSystemTrigger MercyReplica;
        }
        
        [SerializeField]
        private DialogueSystemTrigger _startReplica;

        [SerializeField]
        private DialogueSystemTrigger _startReplica2;

        [SerializeField]
        private GameObject _herobrine;
        
        [SerializeField]
        private GameObject _herobrineCutscene;
        
        [SerializeField]
        private StartBattle_Herobrine _startBattleHerobrine;
        
        [SerializeField]
        private GameObject _timer;
        
        [SerializeField]
        private TMP_Text _timerLabel;

        [SerializeField]
        private GameObject _barrier;
        
        [SerializeField]
        private TMP_Text _barrierLabel;

        [SerializeField]
        private SpriteRenderer _barrierItemIcon;
        
        [SerializeField]
        private Sprite _barrierSword;
        
        [SerializeField]
        private Sprite _barrierArmor;
        
        [SerializeField]
        private Enemy _banana;

        [SerializeField]
        private Enemy _dimas;
        
        [SerializeField]
        private Enemy _juliana;
        
        [SerializeField]
        private Enemy _troll;
        
        [SerializeField]
        private Enemy _hacker;
        
        [SerializeField]
        private Enemy _frost;

        private MainRepositoryStorage _mainRepositoryStorage;
        private GameStateController _gameStateController;
        private MainInventory _mainInventory;

        [Inject]
        private void Construct(MainRepositoryStorage mainRepositoryStorage, GameStateController gameStateController, 
            MainInventory mainInventory)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
            _gameStateController = gameStateController;
            _mainInventory = mainInventory;
        }
        
        private void Start()
        {
            var currentState = State.START;
            
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA_SAVE_KEY, out Data data))
            {
                currentState = data.State;
            }
            
            switch (currentState)
            {
                case State.START:
                    StartCoroutine(AwaitStartCutscene());
                    break;
                case State.BANANA:
                    _banana.StartBattle.gameObject.SetActive(true);
                    break;
                case State.DIMAS:
                    if (_mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent) && attackComponent.Attack < 5)
                    {
                        Debug.Log(attackComponent.Attack);
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierSword;
                        _barrierLabel.text = "Купите КАМ.МЕЧ чтобы продолжить";
                        break;
                    }

                    _dimas.StartBattle.gameObject.SetActive(true);
                    break;
                case State.JULIANA:
                    _juliana.StartBattle.gameObject.SetActive(true);
                    break;
                case State.TROLL:
                    _troll.StartBattle.gameObject.SetActive(true);
                    break;
                case State.FROST:
                    _frost.StartBattle.gameObject.SetActive(true);
                    break;
                case State.HACKER:
                    _hacker.StartBattle.gameObject.SetActive(true);
                    break;
                case State.HEROBRINE:
                    _startBattleHerobrine.gameObject.SetActive(true);
                    break;
                case State.END:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator AwaitStartCutscene()
        {
            _mainRepositoryStorage.Set(SaveConstants.PVPARENA_SAVE_KEY, new Data() { State = State.BANANA });
            _herobrine.SetActive(false);
            _herobrineCutscene.SetActive(true);
            _startReplica.OnUse();
            
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _gameStateController.OpenCutscene();
            
            yield return new WaitForSeconds(1);
            _banana.GameObject.SetActive(false);
            _banana.StartBattle.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1);
            _startReplica2.OnUse();
            
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _gameStateController.OpenCutscene();
            
            yield return new WaitForSeconds(1);
            _herobrine.SetActive(true);
            _herobrineCutscene.SetActive(false);
            _gameStateController.CloseCutscene();
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
        }

        public IEnumerator AwaitStartCutsceneWinBanana(bool isKilled)
        {
            Debug.Log("KillBanana");
            // Речь Херобрина
            if (isKilled)
            {
                _banana.KillReplica.OnUse();
            }
            else
            {
                _banana.MercyReplica.OnUse();
            }
            
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            // Обратный отчет
            yield return AwaitTimer();

            if (_mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent) && attackComponent.Attack < 5)
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите КАМ.МЕЧ чтобы продолжить";
                yield break;
            }
            
            // Спавн следующего противника
            _dimas.GameObject.SetActive(false);
            _dimas.StartBattle.gameObject.SetActive(true);
        }
    }
}