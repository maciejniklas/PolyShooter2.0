using Patterns;
using Patterns.Implementations;
using UnityEngine;

namespace UI
{
    public class LobbyPageStateMonoBehaviour : StateMonoBehaviour
    {
        public override void PrepareState()
        {
            gameObject.SetActive(true);
        }

        public override void DestroyState()
        {
            gameObject.SetActive(false);
        }
    }
}