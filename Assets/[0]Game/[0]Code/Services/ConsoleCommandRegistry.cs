using System;
using I2.Loc;
using QFSW.QC;
using UnityEngine;
using Zenject;

namespace Game
{
    // Регистрация команд для игровой консоли
    public sealed class ConsoleCommandRegistry
    {
        private static ConsoleCommandRegistry _instance;
        private static DiContainer _container => _instance._localDIContainer;
        
        private readonly DiContainer _localDIContainer;

        public ConsoleCommandRegistry(DiContainer localDiContainer)
        {
            _instance = this;

            _localDIContainer = localDiContainer;
        }
        
        [Command()]
        public static void PlayerActivate(bool isActivate)
        {
            _container.Resolve<Player>().gameObject.SetActive(isActivate);
        }
        
        [Command()]
        public static void SetLocale(string lang)
        {
            //CorrectLang.OnСhangeLang(lang);
        }

        [Command()]
        public static void GetLocale()
        {
            Debug.Log($"System: {LocalizationManager.CurrentLanguage}");
        }
        
        [Command()]
        public static void SwitchLocation(string id)
        {
            _container.Resolve<LocationsManager>().SwitchLocation(id, 0);
        }
        
        [Command()]
        public static void SwitchLocation(string id, int point)
        {
            _container.Resolve<LocationsManager>().SwitchLocation(id, point);
        }
        
        [Command()]
        public static void GameStatus()
        {
            Debug.Log(_container.Resolve<GameStateController>().GetState);
        }
        
        [Command()]
        public static void SetGameStatus(GameState status)
        {
            var gameStateController = _container.Resolve<GameStateController>();
            
            switch (status)
            {
                case GameState.OFF:
                    break;
                case GameState.PLAYING:
                    switch (gameStateController.GetState)
                    {
                        case GameState.TRANSITION:
                            gameStateController.EndTransition();
                            break;
                        case GameState.OFF:
                        case GameState.PLAYING:
                        case GameState.MAIN_MENU:
                        case GameState.SHOP:
                        case GameState.ADS:
                        case GameState.ENDER_CHEST:
                            gameStateController.CloseEnderChest();
                            break;
                    }
                    
                    break;
                case GameState.TRANSITION:
                    break;
                case GameState.SHOP:
                    break;
                case GameState.ADS:
                    break;
                case GameState.ENDER_CHEST:
                    gameStateController.OpenEnderChest();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }
    }
}