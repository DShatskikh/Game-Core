using System.Collections;
using System.Linq;
using PixelCrushers.DialogueSystem;
using QFSW.QC.Actions;
using UnityEngine.SceneManagement;

namespace Game
{
    // Регистрация LUA команд
    public sealed class LuaCommandRegister
    {
        private readonly ScreenManager _screenManager;
        private readonly IGameRepositoryStorage _mainRepositoryStorage;

        public LuaCommandRegister(ScreenManager screenManager, IGameRepositoryStorage mainRepositoryStorage)
        {
            _screenManager = screenManager;
            _mainRepositoryStorage = mainRepositoryStorage;
            
            Register();
        }
        
        private void Register()
        {
            Lua.RegisterFunction(nameof(OpenSaveScreen), this,
                SymbolExtensions.GetMethodInfo(() => OpenSaveScreen()));
            
            Lua.RegisterFunction(nameof(IsDefeatedEnemy), this,
                SymbolExtensions.GetMethodInfo(() => 
                    IsDefeatedEnemy(string.Empty)));
            
            Lua.RegisterFunction(nameof(IsKilledEnemy), this,
                SymbolExtensions.GetMethodInfo(() => IsKilledEnemy(string.Empty)));
            
            Lua.RegisterFunction(nameof(IsPassedPVPArena), this,
                SymbolExtensions.GetMethodInfo(() => IsPassedPVPArena()));
            
            Lua.RegisterFunction(nameof(OpenOutro), this,
                SymbolExtensions.GetMethodInfo(() => OpenOutro()));
        }

        private void OpenSaveScreen()
        {
            if (GameplayInstaller.GetContainer != null)
                _screenManager.Open(ScreensEnum.SAVE, GameplayInstaller.GetContainer);
            else
                _screenManager.Open(ScreensEnum.SAVE);
        }

        private bool IsDefeatedEnemy(string enemyID)
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.KILLED_ENEMIES, 
                    out DefeatedEnemiesSaveData defeatedEnemiesSaveData))
                return defeatedEnemiesSaveData.DefeatedEnemies.Any(x => x == enemyID);

            return false;
        }

        private bool IsKilledEnemy(string enemyID)
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.KILLED_ENEMIES, out DefeatedEnemiesSaveData defeatedEnemiesSaveData))
                return defeatedEnemiesSaveData.KilledEnemies.Any(x => x == enemyID);

            return false;
        }

        private bool IsPassedPVPArena()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.SaveData data))
            {
                return data.State == PVPArena.State.END;
            }

            return false;
        }

        private void OpenOutro()
        {
            CoroutineRunner.Instance.StartCoroutine(AwaitOpenOutro());
        }

        private IEnumerator AwaitOpenOutro()
        {
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            SceneManager.LoadScene(3);
        }
    }
}