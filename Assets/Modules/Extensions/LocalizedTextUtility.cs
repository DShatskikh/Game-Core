using System;
using System.Collections;
using UnityEngine.Localization;

namespace Game
{
    public static class LocalizedTextUtility
    {
        public static IEnumerator AwaitLoad(this LocalizedString localizedString, Action<string> onComplete)
        {
            var textOperation = localizedString.GetLocalizedStringAsync();
                
            while (!textOperation.IsDone)
                yield return null;

            string t = textOperation.Result;
            onComplete.Invoke(t);
        }

        /*public static UniTask Load(this LocalizedString localizedString, Action<string> onComplete)
        {
            var text = localizedString.GetLocalizedStringAsync();
            onComplete.Invoke(text.Result);
            //ServiceLocator.Get<CoroutineRunner>().StartCoroutine(AwaitLoad(localizedString, onComplete));
        }*/
    }
}