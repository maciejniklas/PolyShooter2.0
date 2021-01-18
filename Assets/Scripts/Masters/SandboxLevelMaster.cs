using System.Collections;
using Characters.Player;
using UnityEngine;

namespace Masters
{
    /// <summary>
    /// Responsible for game initialization and control fundamental game features
    /// </summary>
    public class SandboxLevelMaster : LevelMaster
    {
        protected override bool IsSandboxAbstractProperty => true;

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!(PlayerModule.LocalPlayer is null)) PlayerModule.LocalPlayer.OnDeath -= Respawn;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            StartCoroutine(AddOnPlayerDeathListener());
        }

        private IEnumerator AddOnPlayerDeathListener()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);

            PlayerModule.LocalPlayer.OnDeath += Respawn;
        }
        
        private void Respawn()
        {
            PlayerModule.LocalPlayer.transform.SetPositionAndRotation(LocalPlayerSpawnPoint.position, LocalPlayerSpawnPoint.rotation);
            PlayerModule.LocalPlayer.RespawnInitialization();
            StartCoroutine(EquipWeaponOnSpawn());
        }
    }
}