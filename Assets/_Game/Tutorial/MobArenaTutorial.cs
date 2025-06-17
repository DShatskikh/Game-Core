using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class MobArenaTutorial : MonoBehaviour
    {
        [SerializeField]
        private MobArena _mobArena;

        [SerializeField]
        private Enemy_WitherSkeleton _witherSkeleton;
        
        [Inject]
        private TutorialState _tutorialState;

        private void Start()
        {
            if (_tutorialState.IsCompleted)
                return;

            _mobArena.HideAllMons();
            
            if (_tutorialState.CurrentStep != TutorialStep.MOVE_MOB_ARENA)
                return;
            
            _witherSkeleton.gameObject.SetActive(true);
        }
    }
}