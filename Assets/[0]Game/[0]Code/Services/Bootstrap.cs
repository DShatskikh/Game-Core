using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public sealed class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene(2);
        }
    }
}