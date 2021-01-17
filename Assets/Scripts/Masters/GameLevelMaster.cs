using System.Collections;
using Characters.Player;
using Photon.Pun;
using UnityEngine;

namespace Masters
{
    /// <summary>
    /// Responsible for game initialization and control fundamental game features
    /// </summary>
    public class GameLevelMaster : LevelMaster
    {
        [Header("Game level")]
        [SerializeField] private GameObject spectatorPrefab;

        protected override bool IsSandboxAbstractProperty => false;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (PlayerModule.LocalPlayer is null) return;
            PlayerModule.LocalPlayer.OnDeath -= MakeSpectatorInstance;
            PlayerModule.LocalPlayer.OnDeath -= DestroyPlayer;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(AddOnPlayerDeathListener());
        }

        private IEnumerator AddOnPlayerDeathListener()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);

            PlayerModule.LocalPlayer.OnDeath += MakeSpectatorInstance;
            PlayerModule.LocalPlayer.OnDeath += DestroyPlayer;
        }

        private static void DestroyPlayer()
        {
            PhotonNetwork.Destroy(PlayerModule.LocalPlayer.gameObject);
        }

        private void MakeSpectatorInstance()
        {
            Instantiate(spectatorPrefab, Vector3.up, Quaternion.identity);
        }
    }
}