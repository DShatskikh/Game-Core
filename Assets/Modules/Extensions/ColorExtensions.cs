using UnityEngine;

namespace Game
{
    // Расширение для работы с цветами
    public static class ColorExtensions
    {
        public static Color SetA(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}