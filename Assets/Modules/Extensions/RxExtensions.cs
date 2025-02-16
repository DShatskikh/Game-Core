using System;
using UniRx;

namespace Game
{
    public static class RxExtensions
    {
        public static IDisposable SubscribeAndCall<T>(this IReactiveProperty<T> source, Action<T> onNext)
        {
            onNext?.Invoke(source.Value);
            return source.Subscribe(onNext);
        }
    }
}