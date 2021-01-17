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
        public event OnUsernameValidationEventHandler OnUsernameValidation;
        public delegate void OnUsernameValidationEventHandler(bool isValid);
        
        public static UsernameInputField Instance { get; private set; }

        private InputField _inputField;
        
        private const string UsernamePrefKey = "PhotonUsername";

        private void Awake()
        {
            // Single scene singleton
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            
            // Reference
            _inputField = GetComponent<InputField>();
        }

        private void Start()
        {
            // Initialize text field of input field
            if (!PlayerPrefs.HasKey(UsernamePrefKey)) return;
            var defaultName = PlayerPrefs.GetString(UsernamePrefKey);
            _inputField.text = defaultName;
            PhotonNetwork.NickName = defaultName;
            OnUsernameValidation?.Invoke(true);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void UpdatePlayerUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                OnUsernameValidation?.Invoke(false);
                Notification.ErrorMessage("Enter valid username!");
                return;
            }

            OnUsernameValidation?.Invoke(true);
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString(UsernamePrefKey, username);
        }
    }
}
