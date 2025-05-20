using PixelCrushers.DialogueSystem;

namespace Game
{
    public sealed class LuaCommandRegister
    {
        private readonly ScreenManager _screenManager;
        private readonly MainRepositoryStorage _mainRepositoryStorage;

        public LuaCommandRegister(ScreenManager screenManager, MainRepositoryStorage mainRepositoryStorage)
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
                SymbolExtensions.GetMethodInfo(() => IsDefeatedEnemy(string.Empty)));
            
            Lua.RegisterFunction(nameof(IsKilledEnemy), this,
                SymbolExtensions.GetMethodInfo(() => IsKilledEnemy(string.Empty)));
            
            Lua.RegisterFunction(nameof(IsPassedPVPArena), this,
                SymbolExtensions.GetMethodInfo(() => IsPassedPVPArena()));
        }

        private void OpenSaveScreen()
        {
            _screenManager.Open(ScreensEnum.SAVE);
        }
        
        private bool IsDefeatedEnemy(string enemyID)
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.KILLED_ENEMIES, out DefeatedEnemiesSaveData defeatedEnemiesSaveData))
            {
                return defeatedEnemiesSaveData.DefeatedEnemies.TryGetValue(enemyID, out var hash);
            }

            return false;
        }
        
        private bool IsKilledEnemy(string enemyID)
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.KILLED_ENEMIES, out DefeatedEnemiesSaveData defeatedEnemiesSaveData))
            {
                return defeatedEnemiesSaveData.KilledEnemies.TryGetValue(enemyID, out var hash);
            }

            return false;
        }
        
        private bool IsPassedPVPArena()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA_SAVE_KEY, out PVPArena.Data data))
            {
                return data.State == PVPArena.State.END;
            }

            return false;
        }
    }
}