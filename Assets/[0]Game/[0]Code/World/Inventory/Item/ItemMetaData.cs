using System;
using I2.Loc;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class ItemMetaData
    {
        public LocalizedString Name;
        public LocalizedString Description;
        public Sprite Icon;

        public ItemMetaData(LocalizedString name, LocalizedString description, Sprite icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }
        
        public ItemMetaData Clone() => 
            new(Name, Description, Icon);
    }
}