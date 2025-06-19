using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика меню
    [Serializable]
    public sealed class MenuPresenter : IScreenPresenter
    {
        private MenuView _view;
        private GameStateController _gameStateController;
        private ScreenManager _screenManager;
        private IAssetLoader _assetLoader;
        private DiContainer _container;

        [Inject]
        private void Construct(MenuView menuView, DiContainer container, GameStateController gameStateController, 
            ScreenManager screenManager, LevelService levelService, HealthService healthService,
            MainInventory mainInventory, WalletService walletService, AttackService attackService, 
            ArmorService armorService, IAssetLoader assetLoader)
        {
            _view = menuView;
            _container = container;
            _gameStateController = gameStateController;
            _screenManager = screenManager;
            _assetLoader = assetLoader;
            
            _gameStateController.OpenCutscene();
            
            _view.GetContinueButton.onClick.AddListener(OnContinueClicked);
            _view.GetInventoryButton.onClick.AddListener(OnInventoryClicked);
            _view.GetStatsButton.onClick.AddListener(OnStatsClicked);
            _view.GetMenuButton.onClick.AddListener(OnMenuClicked);
            EventSystem.current.SetSelectedGameObject(_view.GetContinueButton.gameObject);
            OnStatsClicked();

            string weaponName = mainInventory.WeaponSlot.HasItem ? mainInventory.WeaponSlot.Item.MetaData.Name : "Отсутствует";
            string armorName = mainInventory.ArmorSlot.HasItem ? mainInventory.ArmorSlot.Item.MetaData.Name : "Отсутствует";
            
            var info = "Нубик\n\n";
            info += $"УР: {levelService.GetLv}\n";
            info += $"ЗДОРОВЬЕ: {healthService.GetHealth}/{healthService.GetMaxHealth}\n\n";
            info += $"АТАКА: {attackService.GetAttack - 1} (+{attackService.GetAttack})     ОПЫТ:  {levelService.GetExp}\n";
            info += $"ЗАЩИТА: {attackService.GetAttack} (+{armorService.GetArmor})   ДО УР: {levelService.GetExpToNextLv}\n\n";
            info += $"ОРУЖИЕ: {weaponName}\n";
            info += $"БРОНЯ: {armorName}\n\n";
            info += $"МОН: {walletService.Money}\n";
            
            _view.SetStatsLabel(info);
        }

        public IScreenPresenter Prototype()
        {
            return new MenuPresenter();
        }

        public void Destroy()
        {
            _view.GetContinueButton.onClick.RemoveListener(OnContinueClicked);
            _view.GetContinueButton.onClick.RemoveListener(OnMenuClicked);
            
            _gameStateController.CloseCutscene();
            Object.Destroy(_view.gameObject);
        }

        private void OnContinueClicked()
        {
            _screenManager.Open(ScreensEnum.INPUT, _container);
            _screenManager.Close(ScreensEnum.MENU);
        }

        private void OnMenuClicked()
        {
            _screenManager.Close(ScreensEnum.MENU);
            _screenManager.Close(ScreensEnum.INPUT);
            _assetLoader.LoadScene(AssetPathConstants.MENU_SCENE_PATH);
        }

        private void OnInventoryClicked()
        {
            _view.ToggleStats(false);
            _view.ToggleInventory(true);
        }

        private void OnStatsClicked()
        {
            _view.ToggleStats(true);
            _view.ToggleInventory(false);
        }
    }
}