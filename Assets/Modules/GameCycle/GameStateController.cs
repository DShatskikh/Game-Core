using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class GameStateController : ITickable, IFixedTickable
    {
        private GameState _gameState;
        
        private readonly List<IGameListener> _listeners = new ();
        private readonly List<IGameTickableListener> _tickables = new ();
        private readonly List<IGameFixedTickableListener> _fixedTickables = new ();
        
        public GameState GetState => _gameState;

        public void AddListener(IGameListener listener) 
        {
            if (listener == null || _listeners.Any(x => x == listener))
                return;
            
            _listeners.Add(listener);
            
            if (listener is IGameTickableListener tickableListener)
                _tickables.Add(tickableListener);
            
            if (listener is IGameFixedTickableListener fixedUpdateListener)
                _fixedTickables.Add(fixedUpdateListener);
        }

        public void RemoveListener(IGameListener listener) 
        {
            if (listener is IGameTickableListener tickableListener)
                _tickables.Remove(tickableListener);
            
            if (listener is IGameFixedTickableListener fixedUpdateListener)
                _fixedTickables.Remove(fixedUpdateListener);
            
            _listeners.Remove(listener);
        }

        public void Tick()
        {
            if (_gameState == GameState.PLAYING)
            {
                var delta = Time.deltaTime;
            
                foreach (var tickable in _tickables) 
                    tickable.Tick(delta);
            }
        }

        public void FixedTick()
        {
            if (_gameState == GameState.PLAYING)
            {
                var delta = Time.fixedDeltaTime;
                
                foreach (var fixedTickable in _fixedTickables)
                    fixedTickable.FixedTick(delta);
            }
        }
        
        public void StartGame() 
        {
            if (_gameState != GameState.OFF && _gameState != GameState.FINISHED)
                return;

            for (int i = 0; i < _listeners.Count; i++)
            {
                if (_listeners[i] is IGameStartListener startListeners) 
                    startListeners.OnStartGame();
            }

            _gameState = GameState.PLAYING;
        }

        public void PauseGame()
        {
            if (_gameState != GameState.PLAYING)
                return;
            
            for (int i = 0; i < _listeners.Count; i++)
            {
                if (_listeners[i] is IGamePauseListener pauseListeners) 
                    pauseListeners.OnPauseGame();
            }

            _gameState = GameState.PAUSED;
        }

        public void ResumeGame()
        {
            if (_gameState != GameState.PAUSED)
                return;
            
            for (int i = 0; i < _listeners.Count; i++)
            {
                if (_listeners[i] is IGameResumeListener resumeListeners) 
                    resumeListeners.OnResumeGame();
            }
            
            _gameState = GameState.PLAYING;
        }

        public void FinishGame()
        {
            if (_gameState != GameState.PAUSED && _gameState != GameState.PLAYING)
                return;
            
            for (int i = 0; i < _listeners.Count; i++)
            {
                if (_listeners[i] is IGameFinishListener finishListeners) 
                    finishListeners.OnFinishGame();
            }

            _gameState = GameState.FINISHED;
            Debug.Log("Game over!");
        }

        public void StartTransition()
        {
            if (_gameState != GameState.PLAYING)
                return;

            _gameState = GameState.TRANSITION;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameTransitionListener transitionListener) 
                    transitionListener.OnStartTransition();
            }
        }

        public void EndTransition()
        {
            if (_gameState != GameState.TRANSITION)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameTransitionListener transitionListener) 
                    transitionListener.OnEndTransition();
            }
        }
        
        public void OpenShop()
        {
            if (_gameState != GameState.PLAYING)
                return;

            _gameState = GameState.SHOP;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameShopListener shopListener) 
                    shopListener.OnOpenShop();
            }
        }

        public void CloseShop()
        {
            if (_gameState != GameState.SHOP)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameShopListener shopListener) 
                    shopListener.OnCloseShop();
            }
        }
        
        public void OpenADS()
        {
            if (_gameState != GameState.PLAYING)
                return;

            _gameState = GameState.ADS;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameADSListener adsListener) 
                    adsListener.OnShowADS();
            }
        }

        public void CloseADS()
        {
            if (_gameState != GameState.ADS)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameADSListener adsListener) 
                    adsListener.OnHideADS();
            }
        }

        public void OpenDialog()
        {
            _gameState = GameState.DIALOGUE;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameDialogueListener dialogueListener) 
                    dialogueListener.OnShowDialogue();
            }
        }

        public void CloseDialog()
        {
            if (_gameState != GameState.DIALOGUE)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameDialogueListener dialogueListener) 
                    dialogueListener.OnHideDialogue();
            }
        }
        
        public void OpenEnderChest()
        {
            _gameState = GameState.ENDER_CHEST;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameEnderChestListener dialogueListener) 
                    dialogueListener.OnOpenEnderChest();
            }
        }

        public void CloseEnderChest()
        {
            if (_gameState != GameState.ENDER_CHEST)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameEnderChestListener dialogueListener) 
                    dialogueListener.OnCloseEnderChest();
            }
        }

        public void StartBattle()
        {
            _gameState = GameState.BATTLE;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameBattleListener battleListener) 
                    battleListener.OnOpenBattle();
            } 
        }
        
        public void CloseBattle()
        {
            if (_gameState != GameState.BATTLE)
                return;

            _gameState = GameState.PLAYING;
            
            foreach (var listener in _listeners)
            {
                if (listener is IGameBattleListener battleListener) 
                    battleListener.OnCloseBattle();
            }
        }
    }
}