using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

namespace Game
{
    public sealed class WinBananaCutscene : MonoBehaviour
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogue1;
        
        [SerializeField]
        private DialogueSystemTrigger _dialogue2;

        [SerializeField]
        private Enemy_Banana _enemyBanana;

        [SerializeField]
        private GameObject _herobrine;

        [SerializeField]
        private PlayableDirector _playableDirector;

        [SerializeField]
        private GameObject _herobrineCutscene;
        
        [SerializeField]
        private GameObject _playerCutscene;

        [SerializeField]
        private GameObject _swordCutscene;

        [SerializeField]
        private Transform _startCutscenePlayerPoint;
        
        private Player _player;
        private MainInventory _inventory;
        private IGameRepositoryStorage _mainRepositoryStorage;
        private TutorialState _tutorialState;
        private GameStateController _gameStateController;

        [Inject]
        private void Construct(Player player, MainInventory inventory, IGameRepositoryStorage mainRepositoryStorage, 
            TutorialState tutorialState, GameStateController gameStateController)
        {
            _player = player;
            _inventory = inventory;
            _mainRepositoryStorage = mainRepositoryStorage;
            _tutorialState = tutorialState;
            _gameStateController = gameStateController;
        }

        private void OnDisable()
        {
            _player.GetMover.Stop();
            _player.GetView.OnSpeedChange(0);
        }

        public void StartCutscene(bool isDeath)
        {
            gameObject.SetActive(true);
            StartCoroutine(AwaitCutscene(isDeath));
        }
        
        private IEnumerator AwaitCutscene(bool isDeath)
        {
            while (Vector2.Distance(_startCutscenePlayerPoint.position, _player.transform.position) > 0.1f)
            {
                _player.GetMover.Move(
                    (_startCutscenePlayerPoint.position - _player.transform.position).normalized, true);

                yield return null;
            }

            _player.transform.position = _startCutscenePlayerPoint.position;
            
            _dialogue1.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);

            _gameStateController.OpenCutscene();
            
            _herobrine.SetActive(false);
            _player.gameObject.SetActive(false);
         
            _herobrineCutscene.SetActive(true);
            _playerCutscene.SetActive(true);
            
            _playableDirector.Play();
            yield return new WaitWhile(() => _playableDirector.state == PlayState.Playing);
            
            _herobrineCutscene.SetActive(false);
            _playerCutscene.SetActive(false);
            _swordCutscene.SetActive(false);
            
            _herobrine.SetActive(true);
            _player.gameObject.SetActive(true);
            
            _dialogue2.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            _gameStateController.OpenCutscene();
            
            _inventory.WeaponSlot.RemoveItem();
            _mainRepositoryStorage.Set(SaveConstants.PVPARENA, new PVPArena.SaveData() { State = PVPArena.State.DIMAS });
            _tutorialState.FinishStep();
            
            _player.Flip(true);
            _player.GetView.OnSpeedChange(1);
            
            do
            {
                _player.GetMover.Move(new Vector2(-1, 0), true);
                yield return null;
            } while (_player.GetMover.IsMove);
        }
    }
}