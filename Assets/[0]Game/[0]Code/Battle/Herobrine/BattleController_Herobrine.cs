using Zenject;

namespace Game
{
    public class BattleController_Herobrine : BattleController
    {
        public sealed class Factory : PlaceholderFactory<BattleController_Herobrine> { }

        public BattleController_Herobrine(Player player)
        {
            
        }
    }
}