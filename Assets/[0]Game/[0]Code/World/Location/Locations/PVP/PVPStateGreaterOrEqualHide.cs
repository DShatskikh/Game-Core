using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class PVPStateGreaterOrEqualHide : MonoBehaviour
    {
        [SerializeField]
        private PVPArena.State _state;

        private MainRepositoryStorage _mainRepositoryStorage;

        [Inject]
        private void Construct(MainRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }
        
        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.Data data))
            {
                if (data.State >= _state)
                    gameObject.SetActive(false);
            }
        }
    }
}