using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gameplay.AI.StrikerAI;

namespace Gameplay.AI.Behaviors
{
    public class CombinedStrikerBehavior : StrikerBehavior
    {
        //Needed if behavior will involve more than 2 strikers
        //private List<StrikerAI> partnerList = new List<StrikerAI>();

        protected StrikerAI behaviorOwner;

        private StrikerAI strikerPartner;

        /// <summary>
        /// Overridable function for CombinedStrikerBehaviors that should be called to find a partner.
        /// </summary>
        public virtual StrikerAI FindPartnerToJoin()
        {
            //if (!HasPartner())
            //{
            //    List<StrikerAI> allStrikers = new List<StrikerAI>(FindObjectsOfType<StrikerAI>());

            //    // Loop through all the strikers in the scene
            //    foreach (StrikerAI availableStriker in allStrikers)
            //    {
            //        // Check if the striker is not the current behavior owner and is not currently possessing the ball
            //        if (availableStriker != behaviorOwner && !availableStriker.GetPossessedBall())
            //        {
            //            // Check if the striker is currently executing the passing behavior
            //            CombinedStrikerBehavior strikerBehavior = availableStriker.GetCurrentBehavior() as CombinedStrikerBehavior;

            //            if (strikerBehavior != null && strikerBehavior.GetType() == typeof(CombinedPassingBehavior))
            //            {
            //                // Check if the striker has not already joined this behavior owner as a partner and is not resting
            //                if (strikerBehavior.GetPartner() == null && availableStriker.GetStrikerMode() != StrikerMode.Resting)
            //                {
            //                    // Found an available partner
            //                    Debug.Log(behaviorOwner?.gameObject.name + " found passing partner " + availableStriker?.gameObject.name);
            //                    strikerPartner = availableStriker;
            //                    SetPartner(strikerPartner);
            //                    return availableStriker;
            //                }
            //            }

            //            if (availableStriker.GetCurrentBehavior() == null || availableStriker.GetCurrentBehavior().IsComplete())
            //            {
            //                // Check if the striker is not resting
            //                if (availableStriker.GetStrikerMode() != StrikerMode.Resting)
            //                {
            //                    // Found an available partner
            //                    Debug.Log(behaviorOwner?.gameObject.name + " found passing partner " + availableStriker?.gameObject.name);
            //                    strikerPartner = availableStriker;
            //                    SetPartner(strikerPartner);
            //                    return availableStriker;
            //                }
            //            }
            //        }
            //    }
            //}
            //// No available partners found
            //return null;
            return null;
        }

        /// <summary>
        /// Defines a partner to join the CombinedStrikerBehavior.
        /// </summary>
        /// <param name="partner">The striker to set as a partner</param>
        public void SetPartner(StrikerAI partner)
        {
            strikerPartner = partner;
            strikerPartner.SetIsPartner(true);
        }

        /// <summary>
        /// Removes the current CombinedStrikerBehavior partner, if the partner is not null.
        /// </summary>
        public void RemovePartner()
        {
            if (strikerPartner)
            {
                strikerPartner.SetIsPartner(false);
                strikerPartner = null;
            }
        }

        /// <summary>
        /// Returns the partner of this current behavior.
        /// </summary>
        /// <returns>the partner of this current behavior.</returns>
        public StrikerAI GetPartner()
        {
            return strikerPartner;
        }

        /// <summary>
        /// Checks if this behavior has a partner.
        /// </summary>
        public bool HasPartner()
        {
            return strikerPartner != null;
        }
    }
}
