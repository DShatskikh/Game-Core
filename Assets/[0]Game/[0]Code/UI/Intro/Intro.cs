using System.Collections;
using I2.Loc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public sealed class Intro : MonoBehaviour
    {
        [SerializeField]
        private Button _skipButton;

        [SerializeField]
        private ShopButton _nextButton;
        
        [SerializeField]
        private Slide[] _slides;

        [SerializeField]
        private LocalizedString _showLocalizedString;
        
        [SerializeField]
        private LocalizedString _nextLocalizedString;
        
        [SerializeField]
        private LocalizedString _playLocalizedString;
        
        private Coroutine _coroutine;
        private bool _isClicked;
        private Slide _currentSlide;

        private void Start()
        {
            _coroutine = StartCoroutine(AwaitShow());
            
            _skipButton.onClick.AddListener(Skip);
            _nextButton.onClick.AddListener(Next);
        }

        private void OnDestroy()
        {
            _skipButton.onClick.RemoveListener(Skip);
            _nextButton.onClick.RemoveListener(Next);
        }

        private void Skip()
        {
            StopCoroutine(_coroutine);
            NextLevel();
        }

        private void Next()
        {
            _isClicked = !_isClicked;

            if (_isClicked)
            {
                _currentSlide.Skip();
            }
        }

        private void NextLevel()
        {
            SceneManager.LoadScene(1);
        }
        
        private IEnumerator AwaitShow()
        {
            foreach (var currentSlide in _slides)
            {
                _currentSlide = currentSlide;
                _isClicked = false;
                _nextButton.GetLabel.SetText(_showLocalizedString);
                
                foreach (var slide in _slides)
                {
                    slide.gameObject.SetActive(false);
                }
                
                currentSlide.gameObject.SetActive(true);

                var timer = currentSlide.GetDelay;
                
                while (timer > 0 && !_isClicked)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                if (currentSlide != _slides[^1])
                {
                    _nextButton.GetLabel.SetText(_nextLocalizedString);   
                }
                else
                {
                    _nextButton.GetLabel.SetText(_playLocalizedString);
                    _skipButton.gameObject.SetActive(false);
                }

                _isClicked = true;
                yield return new WaitUntil(() => !_isClicked);
            }

            NextLevel();
        }
    }
}