﻿using System;
using QFSW.QC;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;

namespace Game
{
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
            //Debug.Log($"YG: {YG2.lang}");
            Debug.Log($"System: {LocalizationSettings.SelectedLocale}");
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
        public static void PlayerState()
        {
            //Debug.Log(_instance._player.IsPause);
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
                        case GameState.PAUSED:
                        case GameState.FINISHED:
                        case GameState.MAIN_MENU:
                        case GameState.SHOP:
                        case GameState.ADS:
                        case GameState.DIALOGUE:
                        case GameState.ENDER_CHEST:
                            gameStateController.CloseEnderChest();
                            break;
                        default:
                            gameStateController.ResumeGame();
                            break;
                    }
                    
                    break;
                case GameState.PAUSED:
                    gameStateController.PauseGame();
                    break;
                case GameState.FINISHED:
                    break;
                case GameState.MAIN_MENU:
                    break;
                case GameState.TRANSITION:
                    break;
                case GameState.SHOP:
                    break;
                case GameState.ADS:
                    break;
                case GameState.DIALOGUE:
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