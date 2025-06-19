using System.Collections;
using DG.Tweening;
using FMODUnity;
using UnityEngine;
using Zenject;

namespace Game
{
    // Щит игрока во время битвы
    public class Shield : MonoBehaviour
    {
        private const string SOUND_PATH = "event:/Звуки/Битва/Щит";
        
        [SerializeField]
        private SpriteRenderer _view;

        [SerializeField]
        private float _delayUse;

        private IShell _shell;
        private bool _isUseCoroutine;
        private Coroutine _coroutine;
        private TurnProgressStorage _turnProgressStorage;
        private int _addedProgress = 5;

        [Inject]
        private void Construct(TurnProgressStorage turnProgressStorage)
        {
            _turnProgressStorage = turnProgressStorage;
        }
        
        private void OnEnable()
        {
            _isUseCoroutine = false;
            _view.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_shell != null && !_isUseCoroutine && _shell.IsAlive)
            {
                _isUseCoroutine = true;
                
                if (_coroutine != null)
                    StopCoroutine(_coroutine);
                
                _coroutine = StartCoroutine(Use());
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out IShell attack)) 
                _shell = attack;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<IShell>() != null) 
                _shell = null;
        }

        private IEnumerator Use()
        {
            RuntimeManager.PlayOneShot(SOUND_PATH);
            _view.gameObject.SetActive(true);
            yield return _view.DOColor(_view.color.SetA(1), 0.4f);
            _turnProgressStorage.AddBattleProgress(_addedProgress);
            yield return new WaitForSeconds(_delayUse);
            yield return new WaitForSeconds(_delayUse);
            yield return _view.DOColor(_view.color.SetA(0), 0.4f);
            _view.gameObject.SetActive(false);
            yield return new WaitForSeconds(_delayUse);

            _isUseCoroutine = false;
        }

        public void SetAddedProgress(int progress) => 
            _addedProgress = progress;
    }
}