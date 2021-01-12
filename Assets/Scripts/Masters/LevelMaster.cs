using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Masters
{
    public class LevelMaster : MonoBehaviour, IOnEventCallback
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

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            // Receive event data
            var receivedData = photonEvent.CustomData as object[];

            // Check what event was raised
            switch (photonEvent.Code)
            {
                case SynchronizeSpawnPointsEventCode:
                {
                    // Obtain spawn point name
                    var spawnPointName = (string) receivedData[0];
                    
                    // Check if spawn point with that name exists
                    if (spawnPointsPool.Any((spawnPoint) => spawnPoint.name == spawnPointName))
                    {
                        // Remove spawn point from the pool
                        var usedSpawnPoint =
                            spawnPointsPool.FirstOrDefault((spawnPoint) => spawnPoint.name == spawnPointName);
                        spawnPointsPool.Remove(usedSpawnPoint);
                    }
                    
                    break;
                }
            }
        }

        private IEnumerator SpawnPlayer()
        {
            // Give short time to receive all buffered events, RPC, etc.
            yield return new WaitForSeconds(spawnWaitTimeInSeconds);

            var randomSpawnPointIndex = Random.Range(0, spawnPointsPool.Count);
            var spawnPoint = spawnPointsPool[randomSpawnPointIndex];
            RaiseSynchronizeSpawnPointsEvent(spawnPoint.name);

            var randomPlayerPrefabIndex = Random.Range(0, playerPrefabsPool.Count);
            var randomPlayerPrefab = playerPrefabsPool[randomPlayerPrefabIndex];

            var playerInstance =
                PhotonNetwork.Instantiate(randomPlayerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        }

        private static void RaiseSynchronizeSpawnPointsEvent(string spawnPointName)
        {
            // Form data to send
            var dataToSend = new object[]
            {
                spawnPointName
            };
            
            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };
            
            var sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(SynchronizeSpawnPointsEventCode, dataToSend, raiseEventOptions, sendOptions);
        }
    }
}