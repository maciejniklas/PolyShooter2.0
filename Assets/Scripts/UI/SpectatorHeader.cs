using System.Collections;
using Characters.Player;
using Masters;
using UnityEngine;

namespace UI
{
    public class SpectatorHeader : MonoBehaviour
    {
        [SerializeField] private GameObject headerText;
        
        private void OnEnable()
        {
            StartCoroutine(AddActivationListener());
        }

        private IEnumerator AddActivationListener()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);
            yield return new WaitUntil(() => LevelMaster.Instance != null);

            if (!LevelMaster.Instance.IsSandbox)
            {
                PlayerModule.LocalPlayer.OnDeath += () => headerText.SetActive(true);
            }
        }
    }
}