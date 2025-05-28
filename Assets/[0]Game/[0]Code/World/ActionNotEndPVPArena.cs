using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game
{
    // Вызывает событие если мы не прошли ПВП арену
    public sealed class ActionNotEndPVPArena : MonoBehaviour
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
            if (_mainRepositoryStorage.TryGet(SaveConstants.PVPARENA, out PVPArena.SaveData data) && data.State == PVPArena.State.END)
                return;

            _event.Invoke();
        }
    }
}