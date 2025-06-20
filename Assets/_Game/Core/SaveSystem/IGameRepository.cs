namespace Game
{
    // Интерфейс для репозитория
    public interface IGameRepository
    {
        public void Set<T>(string key, T data);
        public bool TryGet<T>(string key, out T data);
        public void Save();
        public void Load();
    }
}