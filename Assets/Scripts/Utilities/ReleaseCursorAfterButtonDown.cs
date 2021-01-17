using UnityEngine;
using Utilities.Cursor;

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
                CursorStateMachine.ChangeState(CursorStateFactory.Accessible);
            }
            else if (Input.GetButtonUp("ReleaseCursor"))
            {
                CursorStateMachine.ChangeState(CursorStateFactory.Locked);
            }
        }
    }
}