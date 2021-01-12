﻿using Photon.Pun;
using Photon.Realtime;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Masters
{
    public class PhotonMaster : MonoBehaviourPunCallbacks
    {
        public static PhotonMaster Instance { get; private set; }

        private static bool _isConnecting;

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

        public override void OnConnectedToMaster()
        {
            Notification.Instance.InfoMessage("Successfully connected to master server.");

            if (!_isConnecting) return;
            PhotonNetwork.JoinRandomRoom();
            _isConnecting = false;
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarning($"Disconnected from server. Caused by: {cause.ToString()}");
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

            PhotonNetwork.CreateRoom(null, new RoomOptions());
        }

        public override void OnLeftRoom()
        {
            Notification.Instance.InfoMessage("Successfully left room.");

            SceneManager.LoadScene((int) SceneType.Lobby);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Notification.Instance.InfoMessage($"{newPlayer.NickName} entered room.");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Notification.Instance.InfoMessage($"{otherPlayer.NickName} left room.");
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
    }
}