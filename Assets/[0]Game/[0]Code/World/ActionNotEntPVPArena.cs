using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game
{
    public sealed class ActionNotEntPVPArena : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _event;

        private MainRepositoryStorage _mainRepositoryStorage;

        [Inject]
        private void Construct(MainRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }
        
        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.Data data) && data.State == PVPArena.State.END)
                return;

            _event.Invoke();
        }
    }
}