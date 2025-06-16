using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game
{
    public sealed class LobbyPortal : MonoBehaviour
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;

        [Inject]
        private IAssetLoader _assetLoader;
        
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
            _assetLoader.LoadScene(AssetPathConstants.OUTRO_SCENE_PATH);
        }
    }
}