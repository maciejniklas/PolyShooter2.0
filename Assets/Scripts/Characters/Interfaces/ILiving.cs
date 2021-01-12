namespace Characters.Interfaces
{
    /// <summary>
    /// Implements behaviour of living creature
    /// </summary>
    public interface ILiving
    {
        float Health { get; }
        float HealthRegenerationPerSecond { get; }
        float MaxHealth { get; }
        float MaxStamina { get; }
        float Stamina { get; }
        float StaminaRegenerationPerSecond { get; }

        void Death();
        void Hurt(float damage);
        void Tire(float effortCost);
        void Regeneration();

        event OnDeathEventHandler OnDeath;
        event OnHealthValueChangedEventHandler OnHealthValueChanged;
        event OnStaminaValueChangedEventHandler OnStaminaValueChanged;
    }
    
    public delegate void OnDeathEventHandler();
    public delegate void OnHealthValueChangedEventHandler(float health);
    public delegate void OnStaminaValueChangedEventHandler(float stamina);
}