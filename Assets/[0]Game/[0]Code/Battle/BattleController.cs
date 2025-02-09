using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    public class BattleController : MonoBehaviour, IBattleController
    {
        [SerializeField]
        private Transform[] _partyPoints;
        
        [SerializeField]
        private Transform[] _enemyPoints;

        [SerializeField]
        private CinemachineVirtualCameraBase _camera;
        
        private ReactiveProperty<float> _progress = new();
        private IBattleIntro _intro;
        private IBattleOutro _outro;
        private AudioClip _theme;
        private Player _player;
        private IEnemyController _enemyController;
        private GameStateController _gameStateController;

        public IReactiveProperty<float> Progress => _progress;
        public IEnemyController EnemyController => _enemyController;
        
        [Inject]
        private void Construct(Player player, GameStateController gameStateController)
        {
            _player = player;
            _gameStateController = gameStateController;
            
            gameStateController.StartBattle();
            _progress.Value = 0;

            _enemyController = new TestEnemyController();
            _outro = null;
            _theme = null;
        }

        private IEnumerator Start()
        {
            yield return null;
            transform.position = _player.transform.position;
            _camera.gameObject.SetActive(true);
            _intro = new BattleIntro(GetSquadOverWorldPositionsData(), 
                GetEnemiesOverWorldPositionsData());
            yield return _intro.WaitIntro();
            MusicPlayer.Play(_theme);
        }

        public IBattleController SetEnemyController(IEnemyController enemyController)
        {
            _enemyController = enemyController;
            return this;
        }

        public IBattleController SetIntro(IBattleIntro intro)
        {
            _intro = intro;
            return this;
        }

        public IBattleController SetOutro(IBattleOutro outro)
        {
            _outro = outro;
            return this;
        }

        public IBattleController SetBattleTheme(AudioClip theme)
        {
            _theme = theme;
            return this;
        }

        public void StartBattle()
        {
            Debug.Log("StartBattle");
        }

        public void PlayerTurn()
        {
            Debug.Log("PlayerTurn");
        }

        public void AddBattleProgress(int progress)
        {
            _progress.Value += progress;
        }
        
        private OverWorldPositionsData[] GetSquadOverWorldPositionsData()
        {
            var squadOverWorldPositionsData = new List<OverWorldPositionsData>
            {
                new(_player.transform, _partyPoints[0])
            };

            var index = 1;
            
            /*foreach (var companion in GameData.CompanionsManager.GetAllCompanions)
            {
                squadOverWorldPositionsData.Add(new OverWorldPositionsData(companion.transform, 
                    SquadPoints[index], companion.GetSpriteRenderer.sprite));
                
                index++;
            }*/

            return squadOverWorldPositionsData.ToArray();
        }

        private OverWorldPositionsData[] GetEnemiesOverWorldPositionsData()
        {
            var enemiesOverWorldPositions = new List<OverWorldPositionsData>();
            var enemies = EnemyController.GetEnemies;
            
            for (var index = 0; index < enemies.Length; index++)
            {
                var enemy = enemies[index];
                enemiesOverWorldPositions.Add(new OverWorldPositionsData(((MonoBehaviour)enemy).transform,
                    _enemyPoints[index]));
            }

            return enemiesOverWorldPositions.ToArray();
        }
    }
}