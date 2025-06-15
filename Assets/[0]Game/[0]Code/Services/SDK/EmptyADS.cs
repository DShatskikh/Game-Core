using System;

namespace Game
{
    public sealed class EmptyADS : IADSService
    {
        public void ShowRewardVideo(string id, Action callback) { }
        public void ShowFullScreen() { }
    }
}