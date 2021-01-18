using Patterns.Implementations;
using UnityEngine;

namespace Utilities.Cursor
{
    public class CursorLockState : State
    {
        public override void DestroyState() { }

        public override void PrepareState()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        }
    }
}