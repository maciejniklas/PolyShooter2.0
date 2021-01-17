using Patterns.Implementations;

namespace UI
{
    public class LobbyScreenState : StateMonoBehaviour
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