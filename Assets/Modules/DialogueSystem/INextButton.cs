using System;
using Cysharp.Threading.Tasks;

namespace Game
{
    public interface INextButton
    {
        void Show(Action action = null);
        UniTask WaitShow(float time = float.PositiveInfinity, Action action = null);
        void Hide();
    }
}