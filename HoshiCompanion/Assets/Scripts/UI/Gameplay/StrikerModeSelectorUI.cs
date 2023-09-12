using Gameplay.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Gameplay.AI.StrikerAI;

namespace UI.Gameplay
{
    public class StrikerModeSelectorUI : MonoBehaviour
    {
        [Header("Striker")]
        [SerializeField] private StrikerAI striker;

        [Header("Buttons")]
        [SerializeField] private Button trainButton;
        [SerializeField] private Button restButton;

        private bool hasSelectedTraining = false;
        private bool hasSelectedResting = false;

        private void Start()
        {
            if (trainButton)
            {
                trainButton.onClick.AddListener(Train);
            }

            if (restButton)
            {
                restButton.onClick.AddListener(Rest);
            }
        }

        private void OnEnable()
        {
            if (!striker) return;
      
            switch (striker.GetStrikerMode()) 
            {
                case StrikerMode.Idle:
                    trainButton.gameObject.SetActive(true);
                    restButton.gameObject.SetActive(false);
                    break;
                case StrikerMode.Training:
                    trainButton.gameObject.SetActive(false);
                    restButton.gameObject.SetActive(true);
                    break;
                case StrikerMode.Resting:
                    trainButton.gameObject.SetActive(true);
                    restButton.gameObject.SetActive(false);
                    break;
            }
        }

        private void OnDisable()
        {
            trainButton.gameObject.SetActive(false);
            restButton.gameObject.SetActive(false);
        }

        private void Train()
        {
            striker?.SetStrikerMode(StrikerMode.Training);

            if (!hasSelectedTraining)
            {
                striker?.SelectBehavior();
                hasSelectedTraining = true;
            }

            trainButton.gameObject.SetActive(false);
            restButton.gameObject.SetActive(true);
        }

        private void Rest()
        {
            striker?.SetStrikerMode(StrikerMode.Resting);

            if (!hasSelectedResting)
            {
                striker?.SelectBehavior();
                hasSelectedResting = true;
            }

            restButton.gameObject.SetActive(false);
            trainButton.gameObject.SetActive(true);
        }
    }

}
