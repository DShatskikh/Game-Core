using System;

namespace Game
{
    // Структура для хранения сохраненных данных о предмете
    [Serializable]
    public struct ProductSaveData
    {
        public string Id;
        public int Counts;
    }
}