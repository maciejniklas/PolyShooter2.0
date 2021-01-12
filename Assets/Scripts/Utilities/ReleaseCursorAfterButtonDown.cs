using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// After button down, release cursor and make visible and hide it after button is up
    /// </summary>
    public class ReleaseCursorAfterButtonDown : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetButtonDown("ReleaseCursor"))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else if (Input.GetButtonUp("ReleaseCursor"))
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}