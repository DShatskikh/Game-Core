﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
        
        public async UniTask WaitShow(float time = float.PositiveInfinity, Action action = null)
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(gameObject);
            onClick.RemoveAllListeners();
            
            onClick.AddListener(() =>
            {
                onClick.RemoveAllListeners();
                gameObject.SetActive(false);
                time = 0;
                action?.Invoke();
            });
            
            while (time > 0)
            {
                time -= Time.deltaTime;
                await UniTask.WaitForEndOfFrame();
            }
        }
    }

    public interface INextButton
    {
        void Show(Action action = null);
        UniTask WaitShow(float time = float.PositiveInfinity, Action action = null);
    }
}