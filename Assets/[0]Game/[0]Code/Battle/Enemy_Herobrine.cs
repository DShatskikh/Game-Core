using UnityEngine;

namespace Game
{
    public class Enemy_Herobrine : MonoBehaviour, IEnemy
    {
        [SerializeField]
        private BattleMessageBox _messageBox;

        [SerializeField]
        private Attack[] _attacks;
        
        public Attack[] GetAttacks => _attacks;
        public BattleMessageBox GetMessageBox => _messageBox;
    }
}