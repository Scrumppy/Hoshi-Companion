using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Managers
{
    /// <summary>
    /// Parent class of all Managers, automatically creates the Singleton Instance for the child classes.
    /// </summary>
    public abstract class Manager<U> : MonoBehaviour where U : MonoBehaviour
    {
        public static U Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this as U;
            }
        }
    }
}
