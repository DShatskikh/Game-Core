using UnityEditor;
using UnityEngine;
using YG;

namespace Game
{
    public sealed class YandexGamesPurchase : IPurchaseService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IGameRepositoryStorage _gameRepository;

        private YandexGamesPurchase(IAnalyticsService analyticsService, IGameRepositoryStorage gameRepository)
        {
            _analyticsService = analyticsService;
            _gameRepository = gameRepository;
            
            YG2.onPurchaseSuccess += PurchaseSuccessEvent;
            YG2.ConsumePurchases();
        }

        ~YandexGamesPurchase()
        {
            YG2.onPurchaseSuccess -= PurchaseSuccessEvent;
        }

        public void BuyPayments(string id)
        {
            YG2.BuyPayments(id);
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