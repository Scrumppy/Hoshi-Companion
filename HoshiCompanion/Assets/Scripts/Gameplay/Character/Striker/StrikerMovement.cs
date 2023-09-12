using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Striker
{
    public class StrikerMovement : MonoBehaviour
    {
        [Header("Settings")]
        private float currentSpeed;
        private float previousSpeed;

        public delegate void SpeedChanged(float newSpeed);
        public event SpeedChanged OnSpeedChanged;


        #region GETTERS & SETTERS
        public void SetSpeed(float value)
        {
            currentSpeed = value;

            OnSpeedChanged?.Invoke(value);
        }

        public float GetSpeed() { return currentSpeed; }
        #endregion
    }
}


