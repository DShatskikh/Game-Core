using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class MobArenaTutorial : MonoBehaviour
    {
        [SerializeField]
        private MobArena _mobArena;

        [SerializeField]
        private StartBattle_WitherSkeleton _startBattleWitherSkeleton;
        
        [Inject]
        private TutorialState _tutorialState;

        private void Start()
        {
            if (_tutorialState.IsCompleted)
                return;

            _mobArena.HideAllMons();
            
            Debug.Log(_tutorialState.CurrentStep);
            
            if (_tutorialState.CurrentStep != TutorialStep.MOVE_MOB_ARENA)
                return;
            
            _startBattleWitherSkeleton.gameObject.SetActive(true);
        }
    }
}