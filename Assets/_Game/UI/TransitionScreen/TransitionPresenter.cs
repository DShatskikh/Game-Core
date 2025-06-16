using System;
using System.Collections;
using Zenject;
using Object = UnityEngine.Object;

namespace Game
{
    // Логика окна перехода
    public sealed class TransitionPresenter : IScreenPresenter
    {
        private TransitionView _view;
        private CoroutineRunner _coroutineRunner;

        [Inject]
        private void Construct(TransitionView view, CoroutineRunner coroutineRunner)
        {
            _view = view;
            _coroutineRunner = coroutineRunner;
        }
        
        public void Show(Action action = null)
        {
            _view.Toggle(true);
            _coroutineRunner.StartCoroutine(AwaitShow(action));
        }

        public void Hide(Action action = null)
        {
            _view.Toggle(true);
            _coroutineRunner.StartCoroutine(AwaitHide(action));
        }

        public IEnumerator AwaitShow(Action action = null)
        {
            yield return _view.PlayShowAnimation();
            _view.Toggle(false);
            action?.Invoke();
        }
        
        public IEnumerator AwaitHide(Action action = null)
        {
            yield return _view.PlayHideAnimation();
            _view.Toggle(false);
            action?.Invoke();
        }
        
        public IScreenPresenter Prototype() => 
            new TransitionPresenter();

        public void Destroy()
        {
            Object.Destroy(_view.gameObject);
        }
    }
}