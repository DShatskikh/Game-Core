using UnityEngine;
using Zenject;

namespace Game
{
    // Класс выключающий игровой обьект если мы не прошли PVPArena.State
    public sealed class PVPStateGreaterOrEqualHide : MonoBehaviour
    {
        [SerializeField]
        private PVPArena.State _state;

        private IGameRepositoryStorage _mainRepositoryStorage;

        [Inject]
        private void Construct(IGameRepositoryStorage mainRepositoryStorage)
        {
            _mainRepositoryStorage = mainRepositoryStorage;
        }
        
        private void Start()
        {
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.SaveData data))
            {
                if (data.State >= _state)
                    gameObject.SetActive(false);
            }
        }
    }
}