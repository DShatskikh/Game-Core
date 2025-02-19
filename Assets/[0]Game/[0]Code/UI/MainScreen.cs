using UnityEngine;

namespace Game
{
    public class MainScreen : MonoBehaviour, IGameDialogueListener, IGameBattleListener
    {
        void IGameDialogueListener.OnShowDialogue() => 
            ToggleActivate(false);

        void IGameDialogueListener.OnHideDialogue() => 
            ToggleActivate(true);

        void IGameBattleListener.OnOpenBattle() => 
            ToggleActivate(false);

        void IGameBattleListener.OnCloseBattle() => 
            ToggleActivate(true);

        private void ToggleActivate(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}