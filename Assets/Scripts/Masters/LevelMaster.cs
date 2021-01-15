using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters.Interfaces;
using Characters.Player;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using Weapons.Interfaces;
using Random = UnityEngine.Random;

namespace Masters
{
    public class LevelMaster : MonoBehaviour, IOnEventCallback
    {
        [SerializeField] private bool isSandbox = false;
        [SerializeField] private GameObject spectator;
        [Tooltip("Used for right receiving all buffered events, RPC, etc.")]
        [SerializeField] private float spawnWaitTimeInSeconds = 1f;
        [SerializeField] private List<GameObject> playerPrefabsPool;
        [SerializeField] private List<Transform> spawnPointsPool;
        [SerializeField] private List<GameObject> startingWeaponsPool;

        public bool IsSandbox => isSandbox;
        
        public static LevelMaster Instance { get; private set; }

        private Transform _spawnPoint;

        private const byte SynchronizeSpawnPointsEventCode = 1;
        private const byte EquipStartingWeaponOnOtherInstancesEventCode = 2;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            StartCoroutine(SpawnPlayer());
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
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
                
                case EquipStartingWeaponOnOtherInstancesEventCode:
                {
                    // Obtain player
                    var playerViewId = (int) receivedData[0];
                    var playerView = PhotonView.Find(playerViewId);
                    var player = playerView.GetComponent<PlayerModule>();
                    
                    // Obtain weapon
                    var weaponViewId = (int) receivedData[1];
                    var weaponView = PhotonView.Find(weaponViewId);
                    var weapon = weaponView.GetComponent<IWeapon>();
                    
                    // Equip weapon
                    player.EquipWeapon(weapon);
                    
                    break;
                }
            }
        }

        private void EquipWeaponOnSpawn(GameObject playerInstance)
        {
            // Get starting weapon
            var randomStartingWeaponIndex = Random.Range(0, startingWeaponsPool.Count);
            var randomStartingWeaponPrefab = startingWeaponsPool[randomStartingWeaponIndex];
            var startingWeaponInstance =
                PhotonNetwork.Instantiate(randomStartingWeaponPrefab.name, randomStartingWeaponPrefab.transform.position, randomStartingWeaponPrefab.transform.rotation);
            
            // Equip starting weapon
            var startingWeapon = startingWeaponInstance.GetComponent<IWeapon>();
            var playerEquipping = playerInstance.GetComponent<IAbleToEquip>();

            if (playerEquipping.EquippedWeapon != null)
            {
                PhotonNetwork.Destroy(playerEquipping.EquippedWeapon.Instance);
            }
            else
            {
                playerEquipping.OnWeaponEquipped += ShootingWeaponHud.Instance.OnWeaponEquip;
            }
            
            playerEquipping.EquipWeapon(startingWeapon);

            // Raise equip starting weapon event
            var playerViewId = playerInstance.GetPhotonView().ViewID;
            var weaponViewId = startingWeapon.Instance.GetPhotonView().ViewID;
            RaiseEquipStartingWeaponEvent(playerViewId, weaponViewId);
        }

        private void OnPlayerDeathInRound()
        {
            Instantiate(spectator, Vector3.up, Quaternion.identity);
            PhotonNetwork.Destroy(PlayerModule.LocalPlayer.gameObject);
        }

        private void Respawn()
        {
            PlayerModule.LocalPlayer.transform.rotation = _spawnPoint.rotation;
            PlayerModule.LocalPlayer.transform.position = _spawnPoint.position;
            PlayerModule.LocalPlayer.Initialize();
            EquipWeaponOnSpawn(PlayerModule.LocalPlayer.gameObject);
        }

        private IEnumerator SpawnPlayer()
        {
            // Give short time to receive all buffered events, RPC, etc.
            yield return new WaitForSeconds(spawnWaitTimeInSeconds);

            // Get spawn point
            var randomSpawnPointIndex = Random.Range(0, spawnPointsPool.Count);
            _spawnPoint = spawnPointsPool[randomSpawnPointIndex];
            RaiseSynchronizeSpawnPointsEvent(_spawnPoint.name);

            // Get player prefab
            var randomPlayerPrefabIndex = Random.Range(0, playerPrefabsPool.Count);
            var randomPlayerPrefab = playerPrefabsPool[randomPlayerPrefabIndex];
            var playerInstance =
                PhotonNetwork.Instantiate(randomPlayerPrefab.name, _spawnPoint.position, _spawnPoint.rotation);

            if (isSandbox)
            {
                playerInstance.GetComponent<PlayerModule>().OnDeath += Respawn;
            }
            else
            {
                playerInstance.GetComponent<PlayerModule>().OnDeath += OnPlayerDeathInRound;
            }

            EquipWeaponOnSpawn(playerInstance);
        }

        private static void RaiseEquipStartingWeaponEvent(int playerViewId, int weaponViewId)
        {
            // Form data to send
            var dataToSend = new object[]
            {
                playerViewId,
                weaponViewId
            };
            
            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };
            
            var sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(EquipStartingWeaponOnOtherInstancesEventCode, dataToSend, raiseEventOptions, sendOptions);
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