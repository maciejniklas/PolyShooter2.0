using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MatchTimer : MonoBehaviour, IOnEventCallback
    {
        [SerializeField] private int timeToEndOfMatchInSeconds;
        [SerializeField] private Text timeText;
        [SerializeField] private Text timesUpText;
        
        private const byte TimesUpEventCode = 5;

        private void Start()
        {
            StartCoroutine(TimerCoroutine());
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != TimesUpEventCode) return;

            timesUpText.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        public void RestoreTimeScale()
        {
            Time.timeScale = 1;
        }

        private void RaiseTimesUpEvent()
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

            PhotonNetwork.RaiseEvent(TimesUpEventCode, null, raiseEventOptions, sendOptions);
        }

        private IEnumerator TimerCoroutine()
        {
            for (var seconds = timeToEndOfMatchInSeconds; seconds > 0; seconds--)
            {
                timeText.text = seconds.ToString();
                yield return new WaitForSeconds(1);

                if (LivingPlayersCounter.Instance.LivingPlayers == 1)
                {
                    break;
                }
            }

            if (PhotonNetwork.IsMasterClient && LivingPlayersCounter.Instance.LivingPlayers != 1)
            {
                RaiseTimesUpEvent();
            }
        }
    }
}