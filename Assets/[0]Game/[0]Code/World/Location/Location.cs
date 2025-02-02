using UnityEngine;

namespace Game
{
    public sealed class Location : MonoBehaviour
    {
        [SerializeField]
        private string _name;
        
        [SerializeField]
        private Transform[] _points;

        public Transform[] Points => _points;
        public string GetName => _name;
    }
}