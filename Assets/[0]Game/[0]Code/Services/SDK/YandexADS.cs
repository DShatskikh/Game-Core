using System;
using YG;

namespace Game
{
    public sealed class YandexADS : IADSService
    {
        public void ShowRewardVideo(string id, Action callback) => 
            YG2.RewardedAdvShow(id, callback);
        
        public void ShowFullScreen() => 
            YG2.SetFullscreen(true);
    }
}