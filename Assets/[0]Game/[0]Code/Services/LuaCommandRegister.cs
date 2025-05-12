using PixelCrushers.DialogueSystem;

namespace Game
{
    public sealed class LuaCommandRegister
    {
        private readonly ScreenManager _screenManager;

        public LuaCommandRegister(ScreenManager screenManager)
        {
            _screenManager = screenManager;

            Register();
        }
        
        public void Register()
        {
            Lua.RegisterFunction(nameof(OpenSaveScreen), this,
                SymbolExtensions.GetMethodInfo(() => OpenSaveScreen()));
        }

        private void OpenSaveScreen()
        {
            _screenManager.Open(ScreensEnum.SAVE);
        }
    }
}