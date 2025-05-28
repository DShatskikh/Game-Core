using System;
using System.Collections;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using Zenject;
using DialogueSystemTrigger = PixelCrushers.DialogueSystem.Wrappers.DialogueSystemTrigger;

namespace Game
{
    // ПВП арена
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
        
        // Структура для сохранений
        [Serializable]
        public struct SaveData
        {
            public State State;
        }

        [Serializable]
        public struct Enemy
        {
            public StarterBattleBase StartBattle;
            public GameObject GameObject;
            public DialogueSystemTrigger EndBattleReplica;
            public GameObject ExplosionEffect;
        }
        
        [SerializeField]
        private DialogueSystemTrigger _startReplica;

        [SerializeField]
        private DialogueSystemTrigger _startReplica2;
        
        [SerializeField]
        private GameObject _herobrineCutscene;
        
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
        private Transform _arrowBanana;
        
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

        [SerializeField]
        private Enemy _herobrine;
        
        private MainRepositoryStorage _mainRepositoryStorage;
        private GameStateController _gameStateController;
        private MainInventory _mainInventory;
        private TutorialState _tutorialState;

        [Inject]
        private void Construct(MainRepositoryStorage mainRepositoryStorage, GameStateController gameStateController, 
            MainInventory mainInventory, TutorialState tutorialState)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
            _gameStateController = gameStateController;
            _mainInventory = mainInventory;
            _tutorialState = tutorialState;
        }
        
        private void Start()
        {
            var currentState = State.START;
            
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out SaveData data))
            {
                currentState = data.State;
            }

            _mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent);
            _mainInventory.WeaponSlot.Item.TryGetComponent(out ArmorComponent armorComponent);
            
            Debug.Log(attackComponent);
            Debug.Log(armorComponent);
            
            switch (currentState)
            {
                case State.START:
                    StartCoroutine(AwaitStartCutscene());
                    break;
                case State.BANANA:
                    _banana.StartBattle.gameObject.SetActive(true);
                    break;
                case State.DIMAS:
                    if (attackComponent is { Attack: < 5 })
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
                    if (armorComponent is { Armor: < 2 })
                    {
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierArmor;
                        _barrierLabel.text = "Купите КОЖ.БРОН чтобы продолжить";
                        break;
                    }
                    
                    _juliana.StartBattle.gameObject.SetActive(true);
                    break;
                case State.TROLL:
                    if (attackComponent is { Attack: < 7 })
                    {
                        Debug.Log(attackComponent.Attack);
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierSword;
                        _barrierLabel.text = "Купите ЖЕЛ.МЕЧ чтобы продолжить";
                        break;
                    }
                    
                    _troll.StartBattle.gameObject.SetActive(true);
                    break;
                case State.FROST:
                    if (armorComponent is { Armor: < 5 })
                    {
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierArmor;
                        _barrierLabel.text = "Купите ЖЕЛ.БРОН чтобы продолжить";
                        break;
                    }
                    
                    _frost.StartBattle.gameObject.SetActive(true);
                    break;
                case State.HACKER:
                    if (attackComponent is { Attack: < 7 })
                    {
                        Debug.Log(attackComponent.Attack);
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierSword;
                        _barrierLabel.text = "Купите ЖЕЛ.МЕЧ чтобы продолжить";
                        break;
                    }
                    
                    _hacker.StartBattle.gameObject.SetActive(true);
                    break;
                case State.HEROBRINE:
                    if (armorComponent is { Armor: < 7 })
                    {
                        _barrier.SetActive(true);
                        _barrierItemIcon.sprite = _barrierArmor;
                        _barrierLabel.text = "Купите АЛМ.БРОН чтобы продолжить";
                        break;
                    }
                    
                    _herobrine.StartBattle.gameObject.SetActive(true);
                    break;
                case State.END:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator AwaitStartCutscene()
        {
            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, new SaveData() { State = State.BANANA });
            _herobrine.GameObject.SetActive(false);
            _herobrineCutscene.SetActive(true);
            _startReplica.OnUse();
            _gameStateController.OpenCutscene();

            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _gameStateController.OpenCutscene();
            
            yield return new WaitForSeconds(1);
            _banana.GameObject.SetActive(false);
            _arrowBanana.gameObject.SetActive(false);
            _banana.StartBattle.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(1);
            _startReplica2.OnUse();
            
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _gameStateController.OpenCutscene();
            
            _herobrine.GameObject.SetActive(true);
            _herobrine.ExplosionEffect.SetActive(true);
            _herobrineCutscene.SetActive(false);
            _arrowBanana.gameObject.SetActive(true);
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

        private IEnumerator AwaitWinEnemyReplica(Enemy enemy)
        {
            enemy.EndBattleReplica.OnUse();

            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            yield return AwaitTimer();
        }

        private void SpawnNextEnemy(Enemy nextEnemy)
        {
            nextEnemy.GameObject.SetActive(false);
            nextEnemy.ExplosionEffect.SetActive(true);
            nextEnemy.StartBattle.gameObject.SetActive(true);
        }
        
        public IEnumerator AwaitStartCutsceneWinBanana()
        {
            var isCurrentStep = _tutorialState.CurrentStep == TutorialStep.BATTLE_BANANA;
            
            if (isCurrentStep) 
                _tutorialState.FinishStep(false);

            yield return AwaitWinEnemyReplica(_banana);
            
            if (isCurrentStep) 
                _tutorialState.NextStep();
            
            if (_mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent) && attackComponent.Attack < 5)
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите КАМ.МЕЧ чтобы продолжить";
                yield break;
            }
            
            SpawnNextEnemy(_dimas);
        }
        
        public IEnumerator AwaitStartCutsceneWinDimas()
        {
            var isCurrentStep = _tutorialState.CurrentStep == TutorialStep.BATTLE_DIMAS;
            Debug.Log(isCurrentStep);   
            Debug.Log(_tutorialState.CurrentStep);   
            
            if (isCurrentStep) 
                _tutorialState.FinishStep(false);

            yield return AwaitWinEnemyReplica(_dimas);
            
            if (isCurrentStep) 
                _tutorialState.NextStep();

            if (!_mainInventory.ArmorSlot.HasItem || (_mainInventory.ArmorSlot.Item.TryGetComponent(out ArmorComponent armorComponent) && armorComponent.Armor < 2))
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите КОЖ.БРОН чтобы продолжить";
                yield break;
            }

            SpawnNextEnemy(_juliana);
        }

        private void OnDestroy()
        {
            bool isDimasDefeat = false;

            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.SaveData saveData))
            {
                isDimasDefeat = saveData.State == State.JULIANA;
            }

            if (_tutorialState.CurrentStep == TutorialStep.BATTLE_DIMAS && isDimasDefeat)
                _tutorialState.NextStep();
        }

        public IEnumerator AwaitStartCutsceneWinJuliana()
        {
            yield return AwaitWinEnemyReplica(_juliana);

            if (_mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent) && attackComponent.Attack < 7)
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите ЖЕЛ.МЕЧ чтобы продолжить";
                yield break;
            }

            SpawnNextEnemy(_troll);
        }
        
        public IEnumerator AwaitStartCutsceneWinTroll()
        {
            yield return AwaitWinEnemyReplica(_troll);

            if (!_mainInventory.ArmorSlot.HasItem || (_mainInventory.ArmorSlot.Item.TryGetComponent(out ArmorComponent armorComponent) && armorComponent.Armor < 5))
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите ЖЕЛ.БРОН чтобы продолжить";
                yield break;
            }

            SpawnNextEnemy(_frost);
        }
        
        public IEnumerator AwaitStartCutsceneWinFrost()
        {
            yield return AwaitWinEnemyReplica(_frost);

            if (_mainInventory.WeaponSlot.Item.TryGetComponent(out AttackComponent attackComponent) && attackComponent.Attack < 10)
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите АЛМ.МЕЧ чтобы продолжить";
                yield break;
            }

            SpawnNextEnemy(_hacker);
        }
        
        public IEnumerator AwaitStartCutsceneWinHacker()
        {
            yield return AwaitWinEnemyReplica(_hacker);

            if (!_mainInventory.ArmorSlot.HasItem || (_mainInventory.ArmorSlot.Item.TryGetComponent(out ArmorComponent armorComponent) && armorComponent.Armor < 10))
            {
                _timer.SetActive(true);
                _timerLabel.text = "Купите АЛМ.БРОН чтобы продолжить";
                yield break;
            }

            SpawnNextEnemy(_herobrine);
        }
        
        public IEnumerator AwaitStartCutsceneWinHerobrine()
        {
            Debug.Log("Вы победили Херобрина");
            _herobrine.EndBattleReplica.OnUse();

            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);

            Debug.Log("Вы можете закончить игру");
            //SpawnNextEnemy(_herobrine);
        }
    }
}