using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Message text displayed at the top left corner of the screen. Available types of notification are information and error. Should be placed inside of Canvas that don't destroy on other scene load.
    /// </summary>
    public class Notification : MonoBehaviour
    {
        [SerializeField] private float timeVisibleInSeconds = 5f;
        [SerializeField] private Text notificationText;
        public Color informationColor = Color.white;
        public Color errorColor = Color.red;

        private bool _isVisible;
        private bool _isRunning;
        private IEnumerator _currentMessageCoroutine;
        private static Notification _instance;

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
            _isVisible = false;
            _isRunning = false;
        }

        private void Start()
        {
            Hide();
        }

        public static void InfoMessage(string message)
        {
            _instance.Message(message, _instance.informationColor);
        }

        public static void ErrorMessage(string message)
        {
            _instance.Message(message, _instance.errorColor);
        }

        private IEnumerator DisplayAtScreen()
        {
            _isRunning = true;

            if (!_isVisible)
            {
                Show();
            }

            yield return new WaitForSeconds(timeVisibleInSeconds);

            _isRunning = false;
            Hide();
        }

        private void Hide()
        {
            _isVisible = false;
            notificationText.gameObject.SetActive(false);
        }

        private void Message(string message, Color color)
        {
            if (_isRunning)
            {
                _isRunning = false;
                if (_currentMessageCoroutine != null) StopCoroutine(_currentMessageCoroutine);
            }

            _currentMessageCoroutine = DisplayAtScreen();
            StartCoroutine(_currentMessageCoroutine);

            notificationText.color = color;
            notificationText.text = message;
        }

        private void Show()
        {
            notificationText.gameObject.SetActive(true);
            _isVisible = true;
        }
    }
}