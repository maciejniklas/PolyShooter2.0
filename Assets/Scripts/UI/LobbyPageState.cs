using Patterns;
using UnityEngine;

namespace UI
{
    public class LobbyPageState : BaseState
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