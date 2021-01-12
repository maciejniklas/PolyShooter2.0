using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Checks if something entered on interaction with trigger
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TriggerSensor : MonoBehaviour
    {
        public event OnTriggerChangeDetectionEventHandler OnTriggerChangeDetection;
        public delegate void OnTriggerChangeDetectionEventHandler(bool isInside);

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerChangeDetection?.Invoke(true);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerChangeDetection?.Invoke(false);
        }
    }
}