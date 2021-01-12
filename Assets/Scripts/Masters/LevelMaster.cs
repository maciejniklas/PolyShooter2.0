using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Masters
{
    public class LevelMaster : MonoBehaviour
    {
        [Tooltip("Used for right receiving all buffered events, RPC, etc.")]
        [SerializeField] private float spawnWaitTimeInSeconds = 1f;
        [SerializeField] private List<GameObject> playerPrefabsPool;
        [SerializeField] private List<Transform> spawnPointsPool;

        private const byte SynchronizeSpawnPointsEventCode = 1;

        private void Start()
        {
            StartCoroutine(SpawnPlayer());
        }

        private IEnumerator SpawnPlayer()
        {
            // Give short time to receive all buffered events, RPC, etc.
            yield return new WaitForSeconds(spawnWaitTimeInSeconds);

            int randomSpawnPointIndex = Random.Range(0, spawnPointsPool.Count);
            Transform spawnPoint = spawnPointsPool[randomSpawnPointIndex];

            int randomPlayerPrefabIndex = Random.Range(0, playerPrefabsPool.Count);
            GameObject randomPlayerPrefab = playerPrefabsPool[randomPlayerPrefabIndex];

            GameObject playerInstance =
                PhotonNetwork.Instantiate(randomPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}