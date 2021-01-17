using System.Collections;
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
        public static UsernameInputField Instance { get; private set; }
        
        public event OnUsernameValidationEventHandler OnUsernameValidation;
        public delegate void OnUsernameValidationEventHandler(bool isValid);

        private InputField _inputField;
        
        private const string UsernamePrefKey = "PhotonUsername";

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
            
            // Reference
            _inputField = GetComponent<InputField>();
        }

        private void Start()
        {
            StartCoroutine(InitializeUsernameInputField());
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
                Notification.ErrorMessage("Enter valid username! It cannot be null.");
                return;
            }

            OnUsernameValidation?.Invoke(true);
            PhotonNetwork.NickName = username;
            PlayerPrefs.SetString(UsernamePrefKey, username);
        }

        private IEnumerator InitializeUsernameInputField()
        {
            yield return new WaitForSeconds(0.5f);
            
            // Initialize text field of input field
            if (!PlayerPrefs.HasKey(UsernamePrefKey))
            {
                OnUsernameValidation?.Invoke(false);
            }
            else
            {
                var defaultName = PlayerPrefs.GetString(UsernamePrefKey);
                _inputField.text = defaultName;
                PhotonNetwork.NickName = defaultName;
                OnUsernameValidation?.Invoke(true);
            }
        }
    }
}
