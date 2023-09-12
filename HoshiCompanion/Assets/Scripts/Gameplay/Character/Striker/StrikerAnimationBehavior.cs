using Gameplay.AI;
using Gameplay.AI.Behaviors;
using Gameplay.Environment.Ball;
using PlayerData;
using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Striker
{
    public class StrikerAnimationBehavior : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] private Animator animator;

        [Header("References")]
        [SerializeField] private StrikerMovement strikerMovement;
        [SerializeField] private PushupBehavior pushupBehavior;
        [SerializeField] private StrikerPass strikerPass;
        [SerializeField] private CharacterInfo strikerInfo;
        [SerializeField] private StrikerCrossReceive strikerCross;

        [Header("Striker")]
        [SerializeField] private StrikerAI striker;

        private bool isLevelUpAnimating;

        private void Start()
        {
            if (pushupBehavior)
            {
                pushupBehavior.OnPushupBehaviorChanged += OnPushupBehaviorChanged;
            }

            if (strikerPass)
            {
                strikerPass.OnPassChanged += OnPassChanged;
            }

            if (strikerInfo)
            {
                PlayerInfoManager.Instance.OnStrikerLevelUpEvent += OnStrikerLevelUp;
            }

            if (strikerCross)
            {
                strikerCross.OnStrikerCrossReceived += OnStrikerCrossReceived;
            }

            strikerMovement.OnSpeedChanged += OnSpeedChanged;
        }

        private void OnDestroy()
        {
            if (pushupBehavior)
            {
                pushupBehavior.OnPushupBehaviorChanged -= OnPushupBehaviorChanged;
            }

            if (strikerPass)
            {
                strikerPass.OnPassChanged -= OnPassChanged;
            }

            if (strikerInfo)
            {
                PlayerInfoManager.Instance.OnStrikerLevelUpEvent -= OnStrikerLevelUp;
            }

            if (strikerCross)
            {
                strikerCross.OnStrikerCrossReceived -= OnStrikerCrossReceived;
            }

            strikerMovement.OnSpeedChanged -= OnSpeedChanged;
        }

        private void OnPushupBehaviorChanged(bool isDoingPushups)
        {
            animator?.SetBool("Pushups", isDoingPushups);
        }

        private void OnSpeedChanged(float newSpeed)
        {
            animator?.SetFloat("Speed", ConvertSpeedToBlendTree(newSpeed));
        }

        private float ConvertSpeedToBlendTree(float speed)
        {
            return speed * 0.1f;
        }

        private void OnPassChanged(bool passed)
        {
            animator?.SetBool("Passing", passed);
        }

        private void OnStrikerLevelUp(StrikerInfoData strikerData)
        {
            if (strikerData == strikerInfo.Data && !isLevelUpAnimating)
            {
                striker.GetStrikerMovement().SetSpeed(0f);
                animator.SetTrigger("StrikerLevelUp");
            }
        }

        private void OnStrikerCrossReceived(bool value)
        {
            animator?.SetBool("Header", value);
        }

        public void OnLevelUpAnimationFinished()
        {
            isLevelUpAnimating = false;
            striker.GetStrikerMovement().SetSpeed(striker.GetCurrentBehavior().GetSpeed());
            animator.SetTrigger("ReturnToLocomotion");
        }
    }
}
