using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;

namespace Masters
{
    public class PhotonMaster : MonoBehaviourPunCallbacks
    {
        public static PhotonMaster Instance { get; private set; }

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
        }

        private void Start()
        {
            // Users are separated from each other by game version
            PhotonNetwork.GameVersion = Application.version;
            // Force players to have synchronized levels
            PhotonNetwork.AutomaticallySyncScene = true;
            
            ConnectAndJoin();
        }

        public override void OnConnectedToMaster()
        {
            Notification.Instance.InfoMessage("Successfully connected to master server.");

            PhotonNetwork.JoinRandomRoom();
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
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Notification.Instance.ErrorMessage($"Failed joining random room. Message: {message}");

            PhotonNetwork.CreateRoom(null, new RoomOptions());
        }

        private void ConnectAndJoin()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }
}