using MoreMountains.Tools;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _label;

        [SerializeField]
        private MMProgressBar _bar;

        [Inject]
        private void Construct(HealthService healthService)
        {
            healthService.GetHealth.Subscribe(value =>
            {
                _label.text = $"{value}/{healthService.GetMaxHealth.Value}";
                _bar.SetBar(value, 0, healthService.GetMaxHealth.Value);
            });
        }
    }
}