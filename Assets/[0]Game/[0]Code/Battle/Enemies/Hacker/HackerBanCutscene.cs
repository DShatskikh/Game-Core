using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Game
{
    // Катсцена конца битвы против Хакера
    public sealed class HackerBanCutscene : MonoBehaviour
    {
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger;
        
        [SerializeField]
        private DialogueSystemTrigger _dialogueSystemTrigger2;

        [SerializeField]
        private Enemy_Hacker _hacker;

        [SerializeField]
        private GameObject _admin;

        [SerializeField]
        private Transform _adminPoint;
        
        [SerializeField]
        private Transform _adminEndPoint;

        [SerializeField]
        private GameObject _adminStartExplosion;
        
        [SerializeField]
        private GameObject _adminExplosion;
        
        [SerializeField]
        private GameObject _hackerExplosion;

        [SerializeField]
        private GameObject _ban;
        
        public void StartCutscene(BattleController_Hacker battleController)
        {
            Debug.Log("Запустили катсцену");
            StartCoroutine(AwaitCutscene(battleController));
        }

        private IEnumerator AwaitCutscene(BattleController_Hacker battleController)
        {
            yield return new WaitForSeconds(1);
            // _adminStartExplosion.SetActive(true);
            _admin.SetActive(true);
            yield return _admin.transform.DOMove(_adminPoint.position, 1).WaitForCompletion();

            _dialogueSystemTrigger.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);

            _hackerExplosion.transform.position = _hacker.transform.position;
            _hackerExplosion.SetActive(true);
            _ban.transform.position = _hacker.transform.position;
            _ban.SetActive(true);
            _hacker.gameObject.SetActive(false);
            
            _dialogueSystemTrigger2.OnUse();
            yield return new WaitUntil(() => !DialogueManager.instance.isConversationActive);
            
            yield return _admin.transform.DOMove(_adminEndPoint.position, 1).WaitForCompletion();
            // _adminExplosion.transform.position = _admin.transform.position;
            // _adminExplosion.SetActive(true);
            _admin.SetActive(false);
            battleController.EndFight().Forget();
        }
    }
}