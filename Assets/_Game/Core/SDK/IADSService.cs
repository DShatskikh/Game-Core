using System;

namespace Game
{
    public interface IADSService
    {
        void ShowRewardVideo(string id, Action callback);
        void ShowFullScreen();
    }
}