using Febucci.UI;
using FMODUnity;
using MoreMountains.Feedbacks;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;

namespace Game
{
    public sealed class TextAnimatorTypewriterEffect : AbstractTypewriterEffect
    {
        [Header("TextAnimator")]
        [SerializeField]
        private TextAnimatorPlayer _textAnimatorPlayer;

        [SerializeField]
        private StudioEventEmitter _studioEventEmitter;
        
        [SerializeField]
        private GameObject _button;
        
        [SerializeField]
        private MMF_Player _iconMmfPlayer;

        [SerializeField]
        private GameObject _hint;

        [SerializeField]
        private GameObject _nicknamePanel;

        [SerializeField]
        private HeartContinueIcon _heartContinueIcon;

        private bool _isPlaying;
        //private PlayerInput _playerInput;
        //private DeviceTypeDetector _deviceTypeDetector;
        private int _indexAudioSource;

        public override bool isPlaying => _isPlaying;

        public override void Start()
        {
            _heartContinueIcon.gameObject.SetActive(false);

            //_textAnimatorPlayer.textAnimator.effectIntensityMultiplier = GetSpeed();
        }

        public override void Stop()
        {
            _heartContinueIcon.gameObject.SetActive(true);
            _isPlaying = false;
            //_textAnimatorPlayer.textAnimator.SetText(string.Empty, false);
        }

        public override void StartTyping(string text, int fromIndex = 0)
        {
            var actorName = DialogueManager.currentConversationState.subtitle.speakerInfo.nameInDatabase;
            var useDisplayName = DialogueManager.masterDatabase.GetActor(actorName).LookupValue("Display Name");
            _nicknamePanel.SetActive(useDisplayName != string.Empty);

            _heartContinueIcon.gameObject.SetActive(false);
            _textAnimatorPlayer.ShowText(text);
            _textAnimatorPlayer.StartShowingText();
            _isPlaying = true;
            //_textAnimatorPlayer.textAnimator.SetText(text, false);
            //_textAnimatorPlayer.textAnimator.text(text, true);
        }

        public override void StopTyping()
        {
            _textAnimatorPlayer.SkipTypewriter();
        }

        private void OnWrite()
        {
            // _audioSources[_indexAudioSource].Play();
            // //_indexAudioSource++;
            //
            // if (_indexAudioSource >= _audioSources.Length) 
            //     _indexAudioSource = 0;
            
            _studioEventEmitter.Play();
        }
        
        public override void Awake()
        {
            base.Awake();
            //_playerInput = ServiceLocator.Get<PlayerInput>();
            //_deviceTypeDetector = ServiceLocator.Get<DeviceTypeDetector>();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            // gameObject.SetActive(true);
            _textAnimatorPlayer.onCharacterVisible.AddListener((c) => OnWrite());
            _textAnimatorPlayer.onTextShowed.AddListener(Stop);
            // _textAnimatorPlayer.onTypewriterStart.AddListener(OnTypewriterStart);
            // _button.SetActive(false);
            
            if (DialogueManager.currentConversationState == null)
                return;
            
            var text = DialogueManager.currentConversationState.subtitle.formattedText.text;
            _textAnimatorPlayer.ShowText(text);
            _textAnimatorPlayer.StartShowingText(false);

            //_playerInput.actions["Submit"].canceled += ShowAllText;
            //_playerInput.actions["Cancel"].canceled += ShowAllText;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            _textAnimatorPlayer.onCharacterVisible.RemoveListener((c) => OnWrite());
            _textAnimatorPlayer.onTextShowed.RemoveListener(Stop);
            // _textAnimatorPlayer.onTypewriterStart.RemoveListener(OnTypewriterStart);
            // gameObject.SetActive(false);

            // if (_playerInput)
            // {
            //     _playerInput.actions["Submit"].canceled -= ShowAllText;
            //     _playerInput.actions["Cancel"].canceled -= ShowAllText;
            // }
        }

        // public override void Start() { }
        //
        // public override void Stop()
        // {
        //     _isPlaying = false;
        //     _button.SetActive(true);
        //     _hint.SetActive(false);
        //     StopTyping();
        // }

        // public override void StartTyping(string text, int fromIndex = 0)
        // {
        //     
        //     // var clipName = DialogueManager.masterDatabase.GetActor(actorName).LookupValue("AudioClip");
        //     // var clipPath = "AudioClips/" + (clipName != "" ? clipName : "snd_txtlan_ch1");
        //     // var clip = Resources.Load<AudioClip>(clipPath);
        //     //
        //     // foreach (var source in _audioSources) 
        //     //     source.clip = clip;
        //
        //     
        //         
        //     //_textAnimatorPlayer.StartShowingText();
        // }
        //
        // public override void StopTyping()
        // {
        //     _button.SetActive(true);
        // }
        
        
        //
        // private void OnTypewriterStart()
        // {
        //     _isPlaying = true;
        //     _button.SetActive(false);
        //     
        //     // if (_deviceTypeDetector.DeviceType == DeviceType.WebPC)
        //     //     _hint.SetActive(true);
        //     
        //     _iconMmfPlayer.PlayFeedbacks();
        // }
        //
        // private void ShowAllText()
        // {
        //     _textAnimatorPlayer.SkipTypewriter();
        // }
    }
}