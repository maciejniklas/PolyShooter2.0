using System;
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

        [SerializeField] private float healthRegenerationPerSecond = 1f;
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float staminaRegenerationPerSecond = 2.5f;
        [SerializeField] private float maxStamina = 100f;

        public float Health { get; private set; }
        public float HealthRegenerationPerSecond => healthRegenerationPerSecond;
        public float MaxHealth => maxHealth;
        public float MaxStamina => maxStamina;
        public float Stamina { get; private set; }
        public float StaminaRegenerationPerSecond => staminaRegenerationPerSecond;
        
        public static PlayerModule LocalPlayer { get; private set; }

        public event OnDeathEventHandler OnDeath;
        public event OnHealthValueChangedEventHandler OnHealthValueChanged;
        public event OnStaminaValueChangedEventHandler OnStaminaValueChanged;

        private void Awake()
        {
            // Parameters initialization
            Health = MaxHealth;
            Stamina = MaxStamina;
            
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
            if (photonView.IsMine)
            {
                InitializeFpp();
            }
        }

        private void Update()
        {
            if (!photonView.IsMine) return;

            if (Health < MaxHealth || Stamina < MaxStamina)
            {
                Regeneration();
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

        public void Hurt(float damage)
        {
            if (!photonView.IsMine) return;

            Health -= damage;

            if (Health < 0) Health = Mathf.Clamp(Health, 0, MaxHealth);
            
            OnHealthValueChanged?.Invoke(Health);

            if (Health == 0) Death();
        }

        public void Tire(float effortCost)
        {
            if (!photonView.IsMine) return;
            
            Stamina -= effortCost;

            if (Stamina < 0) Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
            
            OnStaminaValueChanged?.Invoke(Stamina);
        }

        public void Regeneration()
        {
            if (!photonView.IsMine) return;
            
            Health += HealthRegenerationPerSecond * Time.deltaTime;
            if (Health > MaxHealth) Health = Mathf.Clamp(Health, 0, MaxHealth);
            OnHealthValueChanged?.Invoke(Health);

            Stamina += StaminaRegenerationPerSecond * Time.deltaTime;
            if (Stamina > MaxStamina) Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
            OnStaminaValueChanged?.Invoke(Stamina);
        }

        private void InitializeFpp()
        {
            visualRepresentation.SetActive(false);
            virtualCamera.SetActive(true);
        }
    }
}