using System.Collections;
using Characters.Interfaces;
using Photon.Pun;
using UnityEngine;

namespace Characters.Player
{
    public class PlayerModule : MonoBehaviourPun, ILiving
    {
        [Header("Initialization")]
        [SerializeField] private GameObject virtualCamera;
        [SerializeField] private GameObject visualRepresentation;

        [Header("Living behaviour")]
        [SerializeField] private float timeToBeSafeInSeconds = 5f;
        [SerializeField] private float healthRegenerationPerSecond = 1f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float timeToRestInSeconds = 5f;
        [SerializeField] private float staminaRegenerationPerSecond = 2.5f;
        [SerializeField] private float maxStamina = 100f;

        public float Health { get; private set; }
        public float HealthRegenerationPerSecond => healthRegenerationPerSecond;
        public bool IsAbleToTire => Stamina > 0;
        public bool IsAfterActivity { get; private set; }
        public bool IsAlive => Health > 0;
        public bool IsTarget { get; private set; }
        public float MaxHealth => maxHealth;
        public float MaxStamina => maxStamina;
        public float Stamina { get; private set; }
        public float StaminaRegenerationPerSecond => staminaRegenerationPerSecond;
        public float TimeToBeSafeInSeconds => timeToBeSafeInSeconds;
        public float TimeToRestInSeconds => timeToRestInSeconds;

        public static PlayerModule LocalPlayer { get; private set; }

        public event OnDeathEventHandler OnDeath;
        public event OnHealthValueChangedEventHandler OnHealthValueChanged;
        public event OnStaminaValueChangedEventHandler OnStaminaValueChanged;

        private IEnumerator _restTimerCoroutine;
        private IEnumerator _safeTimerCoroutine;

        private void Awake()
        {
            // Parameters initialization
            Health = MaxHealth;
            Stamina = MaxStamina;
            IsAfterActivity = false;
            IsTarget = false;

            _restTimerCoroutine = RestTimer();
            _safeTimerCoroutine = SafeTimer();
            
            if (!photonView.IsMine) return;
            
            // Singleton
            if (LocalPlayer != null)
            {
                Destroy(gameObject);
            }
            else
            {
                LocalPlayer = this;
            }
        }

        private void Start()
        {
            if (!photonView.IsMine) return;
            
            // Initialize FPP camera
            visualRepresentation.SetActive(false);
            virtualCamera.SetActive(true);
                
            // Initialize ILiving HUD
            OnHealthValueChanged?.Invoke(Health);
            OnStaminaValueChanged?.Invoke(Stamina);
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            // Testing features
            if (Application.isEditor || Debug.isDebugBuild)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    Hurt(10);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    Tire(10);
                }
            }

            if (!IsAlive) return;
            
            // Regeneration
            if (Health < MaxHealth && !IsTarget)
            {
                HealthRegeneration();
            }

            if (Stamina < MaxStamina && !IsAfterActivity)
            {
                StaminaRegeneration();
            }
        }

        private void OnDestroy()
        {
            if (!photonView.IsMine) return;
            
            // Destroy singleton reference
            if (LocalPlayer == this)
            {
                LocalPlayer = null;
            }
        }
        
        public void Death()
        {
            if (!photonView.IsMine) return;
            
            OnDeath?.Invoke();
        }

        public void HealthRegeneration()
        {
            Health += HealthRegenerationPerSecond * Time.deltaTime;
            if (Health > MaxHealth) Health = Mathf.Clamp(Health, 0, MaxHealth);
            OnHealthValueChanged?.Invoke(Health);
        }

        public void Hurt(float damage)
        {
            if (!photonView.IsMine) return;

            // Update health value
            Health -= damage;

            // Clamp value if over the range
            if (Health < 0) Health = Mathf.Clamp(Health, 0, MaxHealth);
            
            // Notify listeners
            OnHealthValueChanged?.Invoke(Health);

            // Die if have no health
            if (Health == 0) Death();

            // Update IsTarget flag
            if (IsTarget)
            {
                StopCoroutine(_safeTimerCoroutine);
                _safeTimerCoroutine = SafeTimer();
            }
            else
            {
                IsTarget = true;
            }
            StartCoroutine(_safeTimerCoroutine);
        }

        public IEnumerator RestTimer()
        {
            yield return new WaitForSeconds(TimeToRestInSeconds);
            IsAfterActivity = false;
            _restTimerCoroutine = RestTimer();
        }

        public IEnumerator SafeTimer()
        {
            yield return new WaitForSeconds(TimeToBeSafeInSeconds);
            IsTarget = false;
            _safeTimerCoroutine = SafeTimer();
        }

        public void StaminaRegeneration()
        {
            Stamina += StaminaRegenerationPerSecond * Time.deltaTime;
            if (Stamina > MaxStamina) Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
            OnStaminaValueChanged?.Invoke(Stamina);
        }

        public void Tire(float effortCost)
        {
            if (!photonView.IsMine) return;
            
            // Update stamina value
            Stamina -= effortCost;

            // Clamp value if over the range
            if (Stamina < 0) Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
            
            // Notify listeners
            OnStaminaValueChanged?.Invoke(Stamina);

            // Update IsAfterActivity flag
            if (IsAfterActivity)
            {
                StopCoroutine(_restTimerCoroutine);
                _restTimerCoroutine = RestTimer();
            }
            else
            {
                IsAfterActivity = true;
            }
            StartCoroutine(_restTimerCoroutine);
        }
    }
}