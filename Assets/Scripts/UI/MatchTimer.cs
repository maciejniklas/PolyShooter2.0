using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MatchTimer : MonoBehaviour
    {
        [SerializeField] private int timeToEndOfMatchInSeconds;
        [SerializeField] private Text timeText;

        public event OnMatchEndEventHandler OnMatchEnd;
        
        public delegate void OnMatchEndEventHandler();
        
        public static MatchTimer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            StartCoroutine(TimerCoroutine());
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private IEnumerator TimerCoroutine()
        {
            for (var seconds = timeToEndOfMatchInSeconds; seconds > 0; seconds--)
            {
                timeText.text = seconds.ToString();
                yield return new WaitForSeconds(1);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                OnMatchEnd?.Invoke();
            }
        }
    }
}