using UnityEngine;

namespace UI
{
    /// <summary>
    /// Make Canvas to not be destroyed on load another scene
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasDontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}