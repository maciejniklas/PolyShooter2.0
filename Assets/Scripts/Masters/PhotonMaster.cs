using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Masters
{
    public class PhotonMaster : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        [SerializeField] private byte maxNumberOfPlayersPerRoom = 5;
        [Tooltip("Time of waiting when the desired number of players is in room")]
        [SerializeField] private int secondsToRoundStart = 5;
        
        public static PhotonMaster Instance { get; private set; }

        private static bool _isConnecting;
        
        private const byte StartRoundCountdownEventCode = 3;

        private void Awake()
        {
            // Singleton
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            // Initialization
            _isConnecting = false;
        }

        private void Start()
        {
            // Users are separated from each other by game version
            PhotonNetwork.GameVersion = Application.version;
            // Force players to have synchronized levels
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Update()
        {
            if (!Debug.isDebugBuild && !Application.isEditor) return;
            if (!Input.GetKeyDown(KeyCode.Minus)) return;
            if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
            RaiseStartRoundCountdownEvent();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnConnectedToMaster()
        {
            Notification.Instance.InfoMessage("Successfully connected to master server.");

            if (!_isConnecting) return;
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            var messageText = $"Disconnected from server. Caused by: {cause.ToString()}";

            if (Notification.Instance != null)
            {
                Notification.Instance.ErrorMessage(messageText);
            }
            else
            {
                Debug.LogWarning(messageText);
            }
        }

        public override void OnJoinedRoom()
        {
            Notification.Instance.InfoMessage("Successfully joined room.");
            
            PhotonNetwork.LoadLevel((int) SceneType.Sandbox);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Notification.Instance.ErrorMessage($"Failed joining random room. Message: {message}");

            PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = maxNumberOfPlayersPerRoom});
        }

        public override void OnLeftRoom()
        {
            const string messageText = "Successfully left room.";

            if (Notification.Instance != null)
            {
                Notification.Instance.ErrorMessage(messageText);
            }
            else
            {
                Debug.LogWarning(messageText);
            }

            SceneManager.LoadScene((int) SceneType.Lobby);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Notification.Instance.InfoMessage($"{newPlayer.NickName} entered room.");

            if (PhotonNetwork.CurrentRoom.PlayerCount != maxNumberOfPlayersPerRoom) return;
            if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
            RaiseStartRoundCountdownEvent();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Notification.Instance.InfoMessage($"{otherPlayer.NickName} left room.");
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == StartRoundCountdownEventCode)
            {
                StartCoroutine(StartRoundCountdown());
            }
        }

        public void ConnectAndJoin()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private IEnumerator StartRoundCountdown()
        {
            Notification.Instance.InfoMessage("All players are in.");

            for (var index = secondsToRoundStart; index > 0; index--)
            {
                yield return new WaitForSeconds(1);
                
                Notification.Instance.InfoMessage($"Round starts in {index} seconds.");
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel((int) SceneType.Round);
            }
        }

        private static void RaiseStartRoundCountdownEvent()
        {
            var raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };
            
            var sendOptions = new SendOptions
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(StartRoundCountdownEventCode, null, raiseEventOptions, sendOptions);
        }
    }
}