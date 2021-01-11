using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Updates typed username at Photon master server
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class UsernameInputField : MonoBehaviour
    {
        public static readonly string UsernamePrefKey = "PhotonUsername";

        private InputField _inputField;

        private void Awake()
        {
            _inputField = GetComponent<InputField>();
        }

        private void Start()
        {
            var defaultName = string.Empty;

            if (PlayerPrefs.HasKey(UsernamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(UsernamePrefKey);
                _inputField.text = defaultName;
            }

            PhotonNetwork.NickName = defaultName;
        }

        public void UpdatePlayerUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                // TODO: Should be changed by the Notification in future
                Debug.LogError("Username field is not valid!");
                return;
            }

            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString(UsernamePrefKey, username);
        }
    }
}
