using System;

namespace Game
{
    [Serializable]
    public struct SpeakData
    {
        public string NameID;
        public string[] ResponceID;

        public SpeakData(string nameID, string[] responceID)
        {
            NameID = nameID;
            ResponceID = responceID;
        }
    }
}