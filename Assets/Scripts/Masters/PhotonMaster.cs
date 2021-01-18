using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.Cursor;

namespace Masters
{
    /// <summary>
    /// Responsible for connection with server and all Photon callbacks
    /// </summary>
    public class PhotonMaster : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        // Class specific
        
        [SerializeField] private byte maxNumberOfPlayersPerRoom = 5;
        [SerializeField] private int secondsToGameStart = 5;

        private static PhotonMaster _instance;
        private static bool _isConnecting;
        
        private const byte StartGameCountdownEventCode = 3;
        
        // Unity messages

        private void Awake()
        {
            // Singleton global
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
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
            // Feature only for testing purpose
            if (!Debug.isDebugBuild && !Application.isEditor) return;
            if (!Input.GetKeyDown(KeyCode.Minus)) return;
            RaiseStartGameCountdownEvent();
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
        
        // Photon callbacks

        public override void OnConnectedToMaster()
        {
            Notification.InfoMessage("Successfully connected to master server.");

            if (!_isConnecting) return;
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            var messageText = $"Disconnected from server. Caused by: {cause.ToString()}";

            try
            {
                Notification.ErrorMessage(messageText);
            }
            catch
            {
                Debug.LogWarning(messageText);
            }
        }

        public override void OnJoinedRoom()
        {
            Notification.InfoMessage("Successfully joined room.");
            
            PhotonNetwork.LoadLevel((int) SceneType.Sandbox);

            CursorStateMachine.ChangeState(CursorStateFactory.Locked);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Notification.ErrorMessage($"Failed joining random room. Message: {message}");

            PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = maxNumberOfPlayersPerRoom});
        }

        public override void OnLeftRoom()
        {
            const string messageText = "Successfully left room.";

            try
            {
                Notification.ErrorMessage(messageText);
            }
            catch
            {
                Debug.LogWarning(messageText);
            }

            SceneManager.LoadScene((int) SceneType.Lobby);

            CursorStateMachine.ChangeState(CursorStateFactory.Accessible);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Notification.InfoMessage($"{newPlayer.NickName} entered room.");

            if (PhotonNetwork.CurrentRoom.PlayerCount != maxNumberOfPlayersPerRoom) return;
            RaiseStartGameCountdownEvent();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Notification.InfoMessage($"{otherPlayer.NickName} left room.");
        }
        
        // Methods

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != StartGameCountdownEventCode) return;
            if (PhotonNetwork.IsMasterClient) PhotonNetwork.CurrentRoom.IsOpen = false;
            StartCoroutine(StartRoundCountdown());
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
            Notification.InfoMessage("All players are in.");

            for (var index = secondsToGameStart; index >= 0; index--)
            {
                yield return new WaitForSeconds(1);
                
                Notification.InfoMessage($"Round starts in {index} seconds.");
            }

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel((int) SceneType.Game);
            }
        }

        private static void RaiseStartGameCountdownEvent()
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

            PhotonNetwork.RaiseEvent(StartGameCountdownEventCode, null, raiseEventOptions, sendOptions);
        }
    }
}