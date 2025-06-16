using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class AssetLoader : IAssetLoader
    {
        private Dictionary<string, AsyncOperationHandle<GameObject>> _loadedAssets = new();

        public void LoadScene(string scenePath)
        {
            Addressables.LoadSceneAsync(scenePath);
        }

        public async UniTask<GameObject> InstantiateAsync(
            string address,
            Transform parent = null,
            bool worldPositionStays = false)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                if (handle.Result != null)
                {
                    return Object.Instantiate(handle.Result, parent, worldPositionStays);
                }
            }

            var loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedAssets[address] = loadHandle;
                return Object.Instantiate(loadHandle.Result, parent, worldPositionStays);
            }

            Debug.LogError($"Failed to load asset at address: {address}");
            return null;
        }

        public async UniTask<GameObject> LoadAssetAsync(string address)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                return handle.Result;
            }

            var loadHandle = Addressables.LoadAssetAsync<GameObject>(address);
            await loadHandle.Task;

            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                _loadedAssets[address] = loadHandle;
                return loadHandle.Result;
            }

            Debug.LogError($"Failed to load asset at address: {address}");
            return null;
        }

        public AsyncOperationHandle<SceneInstance> LoadSceneAsync(string address, LoadSceneMode loadMode = LoadSceneMode.Single,
            bool activateOnLoad = true, int priority = 100)
        {
            return Addressables.LoadSceneAsync(address, loadMode, activateOnLoad, priority);
        }

        public void ReleaseAsset(string address)
        {
            if (_loadedAssets.TryGetValue(address, out var handle))
            {
                Addressables.Release(handle);
                _loadedAssets.Remove(address);
            }
        }

        public void ReleaseAll()
        {
            foreach (var handle in _loadedAssets.Values)
            {
                Addressables.Release(handle);
            }

            _loadedAssets.Clear();
        }
    }
}