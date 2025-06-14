using System.Collections;
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
            if (!other.GetComponent<Player>())
                return;

            StartCoroutine(AwaitOpenOutro());
        }

        private IEnumerator AwaitOpenOutro()
        {
            _dialogueSystemTrigger.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            SceneManager.LoadScene(3);
        }
    }
}