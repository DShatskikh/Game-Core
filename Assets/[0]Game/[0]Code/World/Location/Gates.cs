using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Gates : MonoBehaviour
    {
        private const string SAVE_HASH = "Gates";
        private const string LAVER1_SAVE_HASH = "TestArena_Laver1";
        private const string LAVER2_SAVE_HASH = "TestArena_Laver2";

        [Serializable]
        public struct Data
        {
            public Data(bool isOpened)
            {
                IsOpened = isOpened;
            }
            
            public bool IsOpened;
        }

        [SerializeField]
        private Transform _upPoint;

        [SerializeField]
        private Transform _bars;

        [SerializeField]
        private Lever _lever;

        [SerializeField]
        private TransitionLocationTrigger _transitionTrigger;

        [SerializeField]
        private StudioEventEmitter _studioEmitter;
        
        private GameStateController _gameStateController;
        private MainRepositoryStorage _mainRepositoryStorage;

        [Inject]
        private void Construct(GameStateController gameStateController, MainRepositoryStorage mainRepositoryStorage)
        {
            _gameStateController = gameStateController;
            _mainRepositoryStorage = mainRepositoryStorage;
            
            _lever.OnUsable += LeverUsable;
        }

        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SAVE_HASH, out Data data) && data.IsOpened)
            {
                _gameStateController.CloseCutscene();
                _transitionTrigger.gameObject.SetActive(true);
                GetComponent<Collider2D>().enabled = false;

                _lever.ActivateNoAnimation();
                _bars.position = _upPoint.position;
            }
        }

        private void OnDestroy()
        {
            _lever.OnUsable -= LeverUsable;
        }

        private void LeverUsable()
        {
            _gameStateController.OpenCutscene();
            var laver1 = false;
            var laver2 = false;

            if (_mainRepositoryStorage.TryGet(LAVER1_SAVE_HASH, out Lever.Data dataLaver1) && dataLaver1.IsActivated)
            {
                laver1 = true;
            }

            if (_mainRepositoryStorage.TryGet(LAVER2_SAVE_HASH, out Lever.Data dataLaver2) && dataLaver2.IsActivated)
            {
                laver2 = true;
            }
            
            if (!laver1 || !laver2)
            {
                StartCoroutine(AwaitReset());
                return;
            }

            _studioEmitter.Play();
            
            _bars
                .DOMoveY(_upPoint.position.y, 2)
                .OnComplete(() =>
                {
                    _gameStateController.CloseCutscene();
                    _transitionTrigger.gameObject.SetActive(true);
                    GetComponent<Collider2D>().enabled = false;
                    
                    _mainRepositoryStorage.Set(SAVE_HASH, new Data(true));
                });
        }

        private IEnumerator AwaitReset()
        {
            yield return new WaitForSeconds(1);
            _lever.Reset();
            _gameStateController.CloseCutscene();
        }
    }
}