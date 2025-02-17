using System.Collections;
using UnityEngine;

namespace Game
{
    public class Enemy_Herobrine : MonoBehaviour, IEnemy
    {
        public void Turn(BattlePresenter presenter)
        {
            StartCoroutine(AwaitTurn(presenter));
        }

        private IEnumerator AwaitTurn(BattlePresenter presenter)
        {
            yield return new WaitForSeconds(1);
            presenter.Turn();
        }
    }
}