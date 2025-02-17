using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Game
{
    public abstract class BattleController : IBattleController
    {
        [SerializeField]
        private Transform[] _partyPoints;
        
        [SerializeField]
        private Transform[] _enemyPoints;

        //private CinemachineVirtualCameraBase _camera;
        
        private ReactiveProperty<float> _progress = new();
        private IBattleIntro _intro;
        private IBattleOutro _outro;
        private AudioClip _theme;
        private Player _player;
        private GameStateController _gameStateController;

        public IReactiveProperty<float> Progress => _progress;
        
        [Inject]
        private void Construct(Player player, GameStateController gameStateController)
        {
            _player = player;
            _gameStateController = gameStateController;
            
            gameStateController.StartBattle();
            _progress.Value = 0;

            _outro = null;
            _theme = null;
        }

        private IEnumerator Start()
        {
            yield return null;
            //transform.position = _player.transform.position;
            //_camera.gameObject.SetActive(true);
            _intro = new BattleIntro(GetSquadOverWorldPositionsData(), 
                GetEnemiesOverWorldPositionsData());
            yield return _intro.WaitIntro();
            MusicPlayer.Play(_theme);
        }


        public IBattleController SetEnemies(IEnemy[] enemies)
        {
            throw new System.NotImplementedException();
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

        public IEnemy[] GetEnemies { get; }

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
            var enemies = GetEnemies;
            
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