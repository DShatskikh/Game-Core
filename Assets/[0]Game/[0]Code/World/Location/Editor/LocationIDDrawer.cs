using UnityEngine;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace Game.Editor
{
    public class LocationIDAttribute : PropertyAttribute
    {
        public string[] options;
        public string[] ids;
    }
    
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LocationIDAttribute))]
    public sealed class LocationIDDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as LocationIDAttribute;
            if (attr.options == null || attr.options.Length == 0)
            {
                LoadLocations(attr);
            }

            // Добавляем пустую строку в начало массивов
            string[] finalOptions = AddEmptyOption(attr.options);
            string[] finalIds = AddEmptyID(attr.ids);

            if (finalOptions != null && finalOptions.Length > 0)
            {
                int selectedIndex = GetIndexByID(finalIds, property.stringValue);
                
                // Выводим Popup с дополненными опциями
                selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, finalOptions);
            
                if (selectedIndex >= 0 && selectedIndex < finalIds.Length)
                {
                    property.stringValue = finalIds[selectedIndex];
                }
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Нет префабов Location в Resources");
            }
        }

        private void LoadLocations(LocationIDAttribute attr)
        {
            var locations = Resources.LoadAll<Location>("");
            attr.options = new string[locations.Length];
            attr.ids = new string[locations.Length];
        
            for (int i = 0; i < locations.Length; i++)
            {
                attr.options[i] = $"{locations[i].name} (ID: {locations[i].GetID})";
                attr.ids[i] = locations[i].GetID;
            }
        }

        private int GetIndexByID(string[] ids, string currentID)
        {
            return Array.IndexOf(ids, currentID);
        }

        // Новые вспомогательные методы
        private string[] AddEmptyOption(string[] original)
        {
            if (original == null) return null;
            
            string[] newArray = new string[original.Length + 1];
            newArray[0] = "Empty";
            Array.Copy(original, 0, newArray, 1, original.Length);
            return newArray;
        }

        private string[] AddEmptyID(string[] original)
        {
            if (original == null) return null;
            
            string[] newArray = new string[original.Length + 1];
            newArray[0] = string.Empty;
            Array.Copy(original, 0, newArray, 1, original.Length);
            return newArray;
        }
    }
#endif
}
