using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class LobbyPortal : MonoBehaviour
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Player player))
            {
                _dialogueSystemTrigger.OnUse();
            }
        }

        public void OpenOutro()
        {
            SceneManager.LoadScene(3);
        }
    }
}