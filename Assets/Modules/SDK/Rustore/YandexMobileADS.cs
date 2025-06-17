using System;

namespace Game
{
    public sealed class YandexMobileADS : IADSService
    {
        private YandexMobileAdsRewardedAd _rewardedAd;
        private YandexMobileAdsInterstitial _interstitialAd;

        public YandexMobileADS()
        {
            _rewardedAd = new YandexMobileAdsRewardedAd();
            _interstitialAd = new YandexMobileAdsInterstitial();
        }
        
        public void ShowRewardVideo(string id, Action callback) => 
            _rewardedAd.ShowRewardVideo(id, callback);

        public void ShowFullScreen() => 
            _interstitialAd.ShowInterstitial();
    }
}