using System.Collections;
using Characters.Interfaces;
using Cinemachine;
using Masters;
using Photon.Pun;
using UnityEngine;
using Weapons.Guns;
using Weapons.Interfaces;
using UnityEngine.Rendering;

namespace Characters.Player
{
    public class PlayerModule : MonoBehaviourPun, ILiving, IAbleToEquip
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

        [Header("Equipping")]
        [Tooltip("On local client weapon is attached to camera")]
        [SerializeField] private Transform localHand;
        [Tooltip("On online clients weapon should be attached to static point")]
        [SerializeField] private Transform onlineHand;
        [SerializeField] private Transform shotSpot;

        public IWeapon EquippedWeapon { get; private set; }
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
        public event OnWeaponEquippedEventHandler OnWeaponEquipped;

        private IEnumerator _restTimerCoroutine;
        private IEnumerator _safeTimerCoroutine;
        private Animator _animator;
        private bool _isDying;
        
        private static readonly int DeathAnimationTrigger = Animator.StringToHash("Death");

        private void Awake()
        {
            // Parameters initialization
            RespawnInitialization();
            _animator = GetComponent<Animator>();

            if (!photonView.IsMine)
            {
                Destroy(virtualCamera.GetComponent<CinemachineVirtualCamera>());
                return;
            }
            
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
            
            // Change model rendering to shadow only
            var modelRenderer = visualRepresentation.GetComponentInChildren<Renderer>();
            modelRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
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

            // Release death trigger
            if (!_isDying)
            {
                _animator.SetTrigger(DeathAnimationTrigger);
                _isDying = true;
            }
            // Called during animation
            else
            {
                OnDeath?.Invoke();
            }
        }
        
        public void EquipWeapon(IWeapon weapon)
        {
            if (EquippedWeapon != null && photonView.IsMine)
            {
                PhotonNetwork.Destroy(EquippedWeapon.Instance);
            }
            
            EquippedWeapon = weapon;
            EquippedWeapon.Owner = gameObject;
            
            EquippedWeapon.Instance.transform.SetParent(photonView.IsMine ? localHand : onlineHand, false);

            var firearm = EquippedWeapon.Instance.GetComponent<Firearm>();
            if (firearm != null) firearm.shotStartPoint = shotSpot;
            
            if (!photonView.IsMine) return;
            
            OnWeaponEquipped?.Invoke(EquippedWeapon);
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
            if (!IsAlive) return;

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

        public void RespawnInitialization()
        {
            Health = MaxHealth;
            Stamina = MaxStamina;
            IsAfterActivity = false;
            IsTarget = false;

            _restTimerCoroutine = RestTimer();
            _safeTimerCoroutine = SafeTimer();
            _isDying = false;
            
            OnHealthValueChanged?.Invoke(Health);
            OnStaminaValueChanged?.Invoke(Stamina);
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

        public void Winner()
        {
            Destroy(virtualCamera.GetComponent<CameraMouseControlModule>());
            Destroy(GetComponent<MovementModule>());
            EquippedWeapon.Instance.transform.SetParent(onlineHand, false);
            var modelRenderer = visualRepresentation.GetComponentInChildren<Renderer>();
            modelRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}