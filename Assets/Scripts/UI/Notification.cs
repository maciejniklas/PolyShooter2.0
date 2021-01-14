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
        [SerializeField] private float visibleTimeInSeconds = 5f;
        [SerializeField] private Text notificationText;
        [SerializeField] private Color informationColor = Color.white;
        [SerializeField] private Color errorColor = Color.red;
        
        public static Notification Instance { get; private set; }

        private bool _isVisible;
        private bool _isRunning;
        private IEnumerator _currentCoroutine;

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
            _isVisible = false;
            _isRunning = false;
        }

        private void Start()
        {
            Hide();
        }

        public void InfoMessage(string message)
        {
            Message(message, informationColor);
        }

        public void ErrorMessage(string message)
        {
            Message(message, errorColor);
        }

        private IEnumerator DisplayAtScreen()
        {
            _isRunning = true;

            if (!_isVisible)
            {
                Show();
            }

            yield return new WaitForSeconds(visibleTimeInSeconds);

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
                if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            }

            _currentCoroutine = DisplayAtScreen();
            StartCoroutine(_currentCoroutine);

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