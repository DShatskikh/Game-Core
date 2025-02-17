using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Shield : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _view;

        [SerializeField]
        private float _delayUse;
        
        [SerializeField]
        private AudioSource _source;
        
        private IShell _shell;
        private bool _isUseCoroutine;
        private Coroutine _coroutine;
        private IBattleController _battleController;

        [Inject]
        private void Construct()
        {
            
        }
        
        private void OnEnable()
        {
            _isUseCoroutine = false;
            _view.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_shell != null && !_isUseCoroutine)
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
            {
                _shell = attack;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent(out IShell attack))
            {
                _shell = null;
            }
        }

        private IEnumerator Use()
        {
            _source.Play();
            _view.gameObject.SetActive(true);
            yield return new WaitForSeconds(_delayUse);
            _view.gameObject.SetActive(false);
            _isUseCoroutine = false;

            _battleController.AddBattleProgress(1);
        }
    }
}