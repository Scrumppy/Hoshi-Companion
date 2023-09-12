using Gameplay.Striker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Gameplay
{
    public class StrikerStaminaBarUI : MonoBehaviour
    {
        [Header("Stamina")]
        [SerializeField] private StrikerStamina strikerStamina;

        [Header("Fill Bar")]
        [SerializeField] private Image fillBar;

        private void Start()
        {
            if (strikerStamina)
            {
                fillBar.fillAmount = strikerStamina.GetStamina();
            }
        }

        private void OnEnable()
        {
            if (strikerStamina)
            {
                strikerStamina.OnStaminaChanged += UpdateStaminaBar;
            }
        }

        private void OnDisable()
        {
            if (strikerStamina)
            {
                strikerStamina.OnStaminaChanged -= UpdateStaminaBar;
            }
        }

        private void UpdateStaminaBar(float value)
        {
            fillBar.fillAmount = strikerStamina.GetCurrentStaminaToFillBar(); 
        }
    }
}

