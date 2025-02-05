using UnityEngine;
using Zenject;

namespace Game
{
    public class GlobalInstaller : MonoInstaller
    {
        [Header("SoundServices")]
        [SerializeField]
        private AudioSource _musicSource;
        
        [SerializeField]
        private AudioSource _soundSource_1;
        
        [SerializeField]
        private AudioSource _soundSource_2;
        
        public override void InstallBindings()
        {
            Container.Bind<VolumeService>().AsSingle();
            
            var musicPlayer = new MusicPlayer(_musicSource);
            var soundPlayer = new SoundPlayer(_soundSource_1, _soundSource_2);
            
            Container.BindInterfacesAndSelfTo<GameStateController>().AsCached();
        }
    }
}