using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game
{
    public interface IAssetLoader
    {
        void LoadScene(string scenePath);
        
        public UniTask<GameObject> InstantiateAsync(
            string address,
            Transform parent = null,
            bool worldPositionStays = false);

        public UniTask<GameObject> LoadAssetAsync(string address);
        
        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(
            [CanBeNull] string address, 
            LoadSceneMode loadMode = LoadSceneMode.Single, 
            bool activateOnLoad = true, int priority = 100);
        
        public void ReleaseAsset(string address);
        public void ReleaseAll();
    }
}