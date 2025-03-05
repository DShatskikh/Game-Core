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
        private void Construct(Heart heart)
        {
            heart.GetHealth.Subscribe(value =>
            {
                _label.text = $"{value}/{heart.GetMaxHealth}";
                _bar.SetBar(value, 0, heart.GetMaxHealth);
            });
        }
    }
}