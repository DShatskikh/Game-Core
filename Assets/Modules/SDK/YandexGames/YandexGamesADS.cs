using System;
using YG;

namespace Game
{
    public sealed class YandexGamesADS : IADSService
    {
        public void ShowRewardVideo(string id, Action callback) => 
            YG2.RewardedAdvShow(id, callback);
        
        public void ShowFullScreen() => 
            YG2.SetFullscreen(true);
    }
}