using MoreMountains.Tools;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;
        
        [SerializeField]
        private MMProgressBar _bar;
        
        [Inject]
        private void Construct(TurnProgressStorage turnProgressStorage)
        {
            turnProgressStorage.Progress.Subscribe(value =>
            {
                _label.text = $"Прогресс хода {value}%";
                _bar.SetBar(value, 0, 100);
            });
        }
    }
}