using System;
using System.Collections;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class Gates : MonoBehaviour
    {
        private const string SAVE_HASH = "Gates";

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

        [Inject]
        private void Construct(GameStateController gameStateController)
        {
            _gameStateController = gameStateController;
            
            _lever.OnUsable += LeverUsable;
        }

        private void Start()
        {
            if (RepositoryStorage.Get<Data>(SAVE_HASH).IsOpened)
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
            
            if (!RepositoryStorage.Get<Lever.Data>("TestArena_Laver1").IsActivated || !RepositoryStorage.Get<Lever.Data>("TestArena_Laver2").IsActivated)
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
                    
                    RepositoryStorage.Set(SAVE_HASH, new Data(true));
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