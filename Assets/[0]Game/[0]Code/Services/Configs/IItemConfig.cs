using UnityEngine;

namespace Game
{
    public interface IItemConfig
    {
        string GetName { get; }
        string GetInfo { get; }
        GameObject Prefab { get; }
    }
}