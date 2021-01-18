using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities
{
    public class SwitchToLobbyIfOffline : MonoBehaviour
    {
        private void Awake()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene((int) SceneType.Lobby);
            }
        }
    }
}