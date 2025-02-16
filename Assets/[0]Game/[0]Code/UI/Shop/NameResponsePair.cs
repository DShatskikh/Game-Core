using System;

namespace Game
{
    [Serializable]
    public struct NameResponsePair
    {
        public string Name;
        public string[] Responces;

        public NameResponsePair(string name, string[] responces)
        {
            Name = name;
            Responces = responces;
        }
    }
}