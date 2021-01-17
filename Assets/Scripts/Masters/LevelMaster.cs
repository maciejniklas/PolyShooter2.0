using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Base class for level masters. It is responsible for game initialization and controlling its fundamental functions
    /// </summary>
    public abstract class LevelMaster : MonoBehaviour, IOnEventCallback
    {
        [Header("General")]
        [SerializeField] private List<Transform> spawnPointsPool;

        public static bool IsSandbox => Instance.IsSandboxAbstractProperty;
        
        protected Transform LocalPlayerSpawnPoint;

        protected abstract bool IsSandboxAbstractProperty { get; }

        private List<GameObject> _playerPrefabsPool;
        private List<GameObject> _startingWeaponsPool;
        
        private static LevelMaster Instance { get; set; }
        
        private const byte SynchronizeSpawnPointsEventCode = 1;
        private const byte EquipStartingWeaponOnOtherInstancesEventCode = 2;

        private void Awake()
        {
            // Singleton local

            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            _playerPrefabsPool = Resources.LoadAll<GameObject>("").Where(prefab => prefab.GetComponent<PlayerModule>() != null).ToList();
            _startingWeaponsPool = Resources.LoadAll<GameObject>("").Where(prefab => prefab.GetComponent<IWeapon>() != null).ToList();
        }

        private void Start()
        {
            SpawnPlayer();
        }

        private void OnDestroy()
        {
            if (Instance != this) return;
            Instance = null;
        }

        protected virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        protected virtual void OnEnable()
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
                    if (receivedData != null)
                    {
                        var spawnPointName = (string) receivedData[0];
                    
                        // Check if spawn point with that name exists
                        if (spawnPointsPool.Any((spawnPoint) => spawnPoint.name == spawnPointName))
                        {
                            // Remove spawn point from the pool
                            var usedSpawnPoint =
                                spawnPointsPool.FirstOrDefault((spawnPoint) => spawnPoint.name == spawnPointName);
                            spawnPointsPool.Remove(usedSpawnPoint);
                        }
                    }

                    break;
                }
                
                case EquipStartingWeaponOnOtherInstancesEventCode:
                {
                    // Obtain player
                    if (receivedData != null)
                    {
                        var playerViewId = (int) receivedData[0];
                        var playerView = PhotonView.Find(playerViewId);
                        var player = playerView.GetComponent<PlayerModule>();
                    
                        // Obtain weapon
                        var weaponViewId = (int) receivedData[1];
                        var weaponView = PhotonView.Find(weaponViewId);
                        var weapon = weaponView.GetComponent<IWeapon>();
                    
                        // Equip weapon
                        player.EquipWeapon(weapon);
                    }

                    break;
                }
            }
        }

        protected IEnumerator EquipWeaponOnSpawn()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);

            // Get starting weapon
            var randomStartingWeaponIndex = Random.Range(0, _startingWeaponsPool.Count);
            var randomStartingWeaponPrefab = _startingWeaponsPool[randomStartingWeaponIndex];
            var startingWeaponInstance =
                PhotonNetwork.Instantiate(randomStartingWeaponPrefab.name, randomStartingWeaponPrefab.transform.position, randomStartingWeaponPrefab.transform.rotation);
            
            // Add on weapon equip listener
            PlayerModule.LocalPlayer.OnWeaponEquipped += ShootingWeaponHud.Instance.OnWeaponEquip;
            
            // Equip starting weapon
            var startingWeapon = startingWeaponInstance.GetComponent<IWeapon>();
            PlayerModule.LocalPlayer.EquipWeapon(startingWeapon);

            // Raise equip starting weapon event
            var playerViewId = PlayerModule.LocalPlayer.photonView.ViewID;
            var weaponViewId = startingWeapon.Instance.GetPhotonView().ViewID;
            RaiseEquipStartingWeaponEvent(playerViewId, weaponViewId);
        }

        private void SpawnPlayer()
        {
            // Get spawn point
            var randomSpawnPointIndex = Random.Range(0, spawnPointsPool.Count);
            LocalPlayerSpawnPoint = spawnPointsPool[randomSpawnPointIndex];
            RaiseSynchronizeSpawnPointsEvent(LocalPlayerSpawnPoint.name);

            // Get player prefab
            var randomPlayerPrefabIndex = Random.Range(0, _playerPrefabsPool.Count);
            var randomPlayerPrefab = _playerPrefabsPool[randomPlayerPrefabIndex];
            PhotonNetwork.Instantiate(randomPlayerPrefab.name, LocalPlayerSpawnPoint.position, LocalPlayerSpawnPoint.rotation);

            StartCoroutine(EquipWeaponOnSpawn());
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