using RuStore.BillingClient;
using UnityEngine;

namespace Game
{
    public sealed class RustorePurchase : IPurchaseService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IGameRepositoryStorage _gameRepository;

        private RustorePurchase(IAnalyticsService analyticsService, IGameRepositoryStorage gameRepository)
        {
            _analyticsService = analyticsService;
            _gameRepository = gameRepository;
        }
        
        public void BuyPayments(string productId)
        {
            RuStoreBillingClient.Instance.PurchaseProduct(
                productId: productId,
                quantity: 1,
                developerPayload: "your payload",
                onFailure: (error) => {
                    // process error
                },
                onSuccess: (result) => {
                    Debug.Log($"Buy: {productId}");

                    // Выдаем покупку
                    switch (productId)
                    {
                        default:
                            break;
                    }
            
                    // Сохраняем игру
                    _gameRepository.Save();
                    _analyticsService.Send("Buy", productId);
                }
            );
        }
    }
}