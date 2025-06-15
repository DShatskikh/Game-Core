using UnityEngine;
using YG;

namespace Game
{
    public sealed class YandexPurchase : IPurchaseService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IGameRepository _gameRepository;

        private YandexPurchase(IAnalyticsService analyticsService, IGameRepository gameRepository)
        {
            _analyticsService = analyticsService;
            _gameRepository = gameRepository;
            
            YG2.onPurchaseSuccess += PurchaseSuccessEvent;
        }

        ~YandexPurchase()
        {
            YG2.onPurchaseSuccess -= PurchaseSuccessEvent;
        }

        // Обрабатываем покупку
        private void PurchaseSuccessEvent(string id)
        {
            Debug.Log($"Buy: {id}");

            // Выдаем покупку
            switch (id)
            {
                default:
                    break;
            }
            
            // Сохраняем игру
            _gameRepository.Save();
            
            _analyticsService.Send("Buy", id);
        }
    }
}