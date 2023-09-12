using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Striker
{
    public class StrikerStamina : MonoBehaviour
    {
        [Header("Stamina Data")]
        [SerializeField] private StrikerStaminaData staminaData;

        [Header("Settings")]
        private float stamina;
        private float maxStamina;
        private bool hasDrainedStamina;
        private bool isExhausted;

        public delegate void StaminaChanged(float newStamina);
        public event StaminaChanged OnStaminaChanged;

        private void Awake()
        {   
            if (staminaData)
            {
                maxStamina = staminaData.stamina;
                stamina = maxStamina;
                hasDrainedStamina = false;
                isExhausted = false;
            }
        }

        /// <summary>
        /// This function drains the striker's stamina overtime by a given value.
        /// </summary>
        /// <param name="amount">The stamina drain amount.</param>
        public void DrainStamina(float amount)
        {
            float amountToDrain = amount * Time.deltaTime;

            stamina -= amountToDrain;

            stamina = Mathf.Clamp(stamina, 0f, maxStamina);

            if (stamina <= 0f)
            {
                stamina = 0f;
                SetExhausted(true);
            }

            OnStaminaChanged?.Invoke(stamina);
        }

        /// <summary>
        /// This function restores the striker's stamina overtime by a given value.
        /// </summary>
        /// <param name="amount">The stamina restore amount.</param>
        public void RestoreStamina(float amount)
        {
            float amountToRestore = amount * Time.deltaTime;

            stamina += amountToRestore;

            stamina = Mathf.Clamp(stamina, 0f, maxStamina);

            if (stamina >= maxStamina)
            {
                stamina = maxStamina;
                SetExhausted(false);
            }

            OnStaminaChanged?.Invoke(stamina);
        }

        #region GETTERS & SETTERS
        public void SetExhausted(bool value)
        {
            isExhausted = value;
        }

        public bool IsExhausted() { return isExhausted; }
        public bool HasDrainedStamina() { return hasDrainedStamina; }
        public float GetStamina() { return stamina; }
        public float GetMaxStamina() { return maxStamina; }
        public float GetExhaustedSpeedModifier() { return staminaData.exhaustedSpeedModifier; }
        public float GetExhaustedTimeModifier() { return staminaData.exhaustedTimeModifier; }
        public float GetCurrentStaminaToFillBar() { return stamina / maxStamina; }
        #endregion
    }
}
