using System;
using Characters.Player;
using Patterns;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Slider))]
    public class LivingPropertySlider : MonoBehaviour
    {
        [SerializeField] private LivingPropertySliderType type;
        
        private Slider _slider;
        private KeepTryingUntilYouSucceed _keepTryingUntilYouSucceed;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _keepTryingUntilYouSucceed = new KeepTryingUntilYouSucceed(0.5f, AddListener);
        }

        private void OnDisable()
        {
            switch (type)
            {
                case LivingPropertySliderType.Health:
                {
                    if (!(PlayerModule.LocalPlayer is null)) PlayerModule.LocalPlayer.OnHealthValueChanged -= UpdateValue;
                    
                    break;
                }
                case LivingPropertySliderType.Stamina:
                {
                    if (!(PlayerModule.LocalPlayer is null)) PlayerModule.LocalPlayer.OnStaminaValueChanged -= UpdateValue;
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnEnable()
        {
            StartCoroutine(_keepTryingUntilYouSucceed.Try());
        }

        private void AddListener()
        {
            switch (type)
            {
                case LivingPropertySliderType.Health:
                {
                    PlayerModule.LocalPlayer.OnHealthValueChanged += UpdateValue;
                    
                    break;
                }
                case LivingPropertySliderType.Stamina:
                {
                    PlayerModule.LocalPlayer.OnStaminaValueChanged += UpdateValue;
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateValue(float value)
        {
            switch (type)
            {
                case LivingPropertySliderType.Health:
                {
                    _slider.value = value / PlayerModule.LocalPlayer.MaxHealth;
                    
                    break;
                }
                case LivingPropertySliderType.Stamina:
                {
                    _slider.value = value / PlayerModule.LocalPlayer.MaxStamina;
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}