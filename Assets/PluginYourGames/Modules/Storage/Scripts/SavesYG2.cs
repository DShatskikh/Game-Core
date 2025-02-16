
using System.Collections.Generic;
using Game;

namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        public List<SerializablePair<string,string>> Container { get; set; }
    }
}
