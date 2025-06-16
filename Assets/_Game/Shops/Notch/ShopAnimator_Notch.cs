using Febucci.UI;
using UnityEngine;
using Zenject;

namespace Game
{
    public class ShopAnimator_Notch : ShopBackground
    {
        [SerializeField]
        private Animator _animator;

        private static readonly int State = Animator.StringToHash("State");

        private void Start()
        {
            var view = FindFirstObjectByType<ShopView>();
            
            view.GetMonologueText.GetComponent<TextAnimatorPlayer>().onTypewriterStart.AddListener(() =>
            {
                //_animator.SetFloat(State, 1);
            });
            
            view.GetMonologueText.GetComponent<TextAnimatorPlayer>().onTextDisappeared.AddListener(() =>
            {
                //_animator.SetFloat(State, 0);
            });
        }
    }
}