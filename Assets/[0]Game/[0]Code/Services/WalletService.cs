using UniRx;

namespace Game
{
    public class WalletService
    {
        public IReactiveProperty<int> Money => _money;
        
        private IntReactiveProperty _money = new();

        public void SetMoney(int money)
        {
            _money.Value = money;
        }

        public void AddMoney(int money)
        {
            _money.Value += money;
        }

        public bool TrySellMoney(int money)
        {
            if (_money.Value >= money)
            {
                _money.Value -= money;
                return true;
            }

            return false;
        }
    }
}