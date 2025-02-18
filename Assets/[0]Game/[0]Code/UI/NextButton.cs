using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class NextButton : Button, INextButton
    {
        public void Show(Action action = null)
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(gameObject);
            onClick.RemoveAllListeners();
            
            onClick.AddListener(() =>
            {
                onClick.RemoveAllListeners();
                gameObject.SetActive(false);
                action?.Invoke();
            });
        }
    }

    public interface INextButton
    {
        void Show(Action action = null);
    }
}