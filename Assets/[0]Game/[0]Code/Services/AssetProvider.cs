using QFSW.QC;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    [CreateAssetMenu(fileName = "AssetProvider", menuName = "Data/AssetProvider", order = 70)]
    public sealed class AssetProvider : ScriptableObject
    {
        public void Init()
        {
            Instance = this;
        }

        public static AssetProvider Instance { get; private set; }

        public QuantumConsole QuantumConsole;
        public EnderChestScreen EnderChestScreen;
        public BattleController BattleController;
    }
}