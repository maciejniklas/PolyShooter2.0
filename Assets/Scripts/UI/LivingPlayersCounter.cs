using System;
using System.Collections;
using Characters.Player;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LivingPlayersCounter : MonoBehaviour, IOnEventCallback
    {
        [SerializeField] private Text livingPlayersText;
        [SerializeField] private GameObject winScreen;
        
        public int LivingPlayers
        {
            get { return int.Parse(livingPlayersText.text); }
        }
        
        public static LivingPlayersCounter Instance { get; private set; }
        
        private const byte DecreaseLivingPlayersEventCode = 4;

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
            if(PhotonNetwork.IsConnected) livingPlayersText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);

            StartCoroutine(AddPlayerDeathListener());
        }

        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code != DecreaseLivingPlayersEventCode) return;

            var livingPlayers = int.Parse(livingPlayersText.text);
            livingPlayers -= 1;
            livingPlayersText.text = livingPlayers.ToString();

            if (livingPlayers == 1)
            {
                LastPlayerWins();
            }
        }

        private IEnumerator AddPlayerDeathListener()
        {
            yield return new WaitUntil(() => PlayerModule.LocalPlayer != null);

            PlayerModule.LocalPlayer.OnDeath += RaiseDecreaseLivingPlayersCounterEvent;
        }

        private void RaiseDecreaseLivingPlayersCounterEvent()
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

            PhotonNetwork.RaiseEvent(DecreaseLivingPlayersEventCode, null, raiseEventOptions, sendOptions);
        }

        private void LastPlayerWins()
        {
            Instantiate(winScreen, Vector3.zero, Quaternion.identity);
            PlayerModule.LocalPlayer.Winner();
        }
    }
}