using Patterns.Implementations;
using UnityEngine;

namespace Utilities.Cursor
{
    public class CursorAccessibleState : State
    {
        public override void DestroyState() { }

        public override void PrepareState()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }
}