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

        private IGameRepositoryStorage _mainRepositoryStorage;

        public struct SaveData { }

        [Inject]
        private void Construct(IGameRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }

        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.SaveData pvpData))
            {
                if (pvpData.State != PVPArena.State.DIMAS)
                    return;
            }
            else
            {
                return;
            }
            
            if (_mainRepositoryStorage.TryGet(SAVE_KEY, out SaveData saveData))
                return;
            
            _action.Invoke();
            _mainRepositoryStorage.Set(SAVE_KEY, new SaveData() { });
        }
    }
}