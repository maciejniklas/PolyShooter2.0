using System.Collections;

namespace Characters.Interfaces
{
    /// <summary>
    /// Implements behaviour of living creature
    /// </summary>
    public interface ILiving
    {
        float Health { get; }
        float HealthRegenerationPerSecond { get; }
        bool IsAbleToTire { get; }
        bool IsAfterActivity { get; }
        bool IsAlive { get; }
        bool IsTarget { get; }
        float MaxHealth { get; }
        float MaxStamina { get; }
        float Stamina { get; }
        float StaminaRegenerationPerSecond { get; }
        float TimeToBeSafeInSeconds { get; }
        float TimeToRestInSeconds { get; }

        void Death();
        void HealthRegeneration();
        void Hurt(float damage);
        IEnumerator RestTimer();
        IEnumerator SafeTimer();
        void StaminaRegeneration();
        void Tire(float effortCost);

        event OnDeathEventHandler OnDeath;
        event OnHealthValueChangedEventHandler OnHealthValueChanged;
        event OnStaminaValueChangedEventHandler OnStaminaValueChanged;
    }
    
    public delegate void OnDeathEventHandler();
    public delegate void OnHealthValueChangedEventHandler(float health);
    public delegate void OnStaminaValueChangedEventHandler(float stamina);
}