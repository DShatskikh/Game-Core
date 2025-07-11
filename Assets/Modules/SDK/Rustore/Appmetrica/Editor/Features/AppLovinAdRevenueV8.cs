using Io.AppMetrica.Editor.Features.Utils;

namespace Io.AppMetrica.Editor.Features {
    internal class AppLovinAdRevenueV8 : Feature {
        public AppLovinAdRevenueV8(string featureName) : base(featureName) {}

        internal override string DefineName => "APPMETRICA_FEATURES_ADREVENUE_APPLOVIN_V8";

        internal override bool IsAutoEnableable => true;

        internal override void AutoEnableFeatureIfAvailable() {
            if (FeatureUtils.IsAssetInProject("MaxSdk")) {
                AutoEnableFeatureIfNeeded();
            }
        }
    }
}
