namespace Game
{
    public sealed class EmptyPurchase : IPurchaseService
    {
        public void BuyPayments(string id) { }
    }
}