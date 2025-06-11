using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game
{
    public sealed class HerobrineTookSwordAction : MonoBehaviour
    {
        private const string SAVE_KEY = "HerobrineTookSwordAction";

        [SerializeField]
        private UnityEvent _action;

        private MainRepositoryStorage _mainRepositoryStorage;

        public struct SaveData { }

        [Inject]
        private void Construct(MainRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }

        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SAVE_KEY, out SaveData saveData))
                return;
            
            _action.Invoke();
            _mainRepositoryStorage.Set(SAVE_KEY, new SaveData() { });
        }
    }
}