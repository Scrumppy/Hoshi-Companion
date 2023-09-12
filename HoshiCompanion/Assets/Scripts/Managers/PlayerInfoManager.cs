using Gameplay.Managers;
using PlayerData.Strikers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    public class PlayerInfoManager : Manager<PlayerInfoManager>
    {
        public PlayerMainInfoData playerMainInfoData;
        public XpNeededToLevelUpData xpNeededToLevelUpStriker;

        #region EVENTS
        public Action<StrikerInfoData> OnStrikerLevelUpEvent = null;
        public Action<StrikerInfoData> OnUpdateStrikerXP = null;
        #endregion

        /// <summary>
        /// This function adds a certain amount of BL to the player.
        /// </summary>
        /// <param name="amountOfBlToAdd">The amount of BL to add.</param>
        public void AddBl(int amountOfBlToAdd)
        {
            playerMainInfoData.bl += amountOfBlToAdd;
            //OnUpdateBl?.Invoke(playerMainInfoData.bl);
        }

        #region Utility functions
        /// <summary>
        /// This function is gives XP to a given striker and increases his level if its the case.
        /// </summary>
        /// <param name="striker_">The striker's info data.</param>
        /// <param name="amountOfXpToIncrease_">The amount of XP to give to the player.<param>
        public void GiveXpToSpecificStriker(StrikerInfoData striker_, float amountOfXpToIncrease_)
        {
            float xp = striker_.ID.currentXp;
            int oldLevel = striker_.Level;
            xp = Mathf.Min(xp + amountOfXpToIncrease_, xpNeededToLevelUpStriker.MaxXP);
            striker_.ID.currentXp = xp;
            OnUpdateStrikerXP?.Invoke(striker_);
            int newLevel = xpNeededToLevelUpStriker.GetLevelFromXP(xp);

            if (oldLevel != newLevel)
            {
                striker_.Level = Mathf.Min(newLevel, xpNeededToLevelUpStriker.MaxLevel);
                if (oldLevel < newLevel)
                {
                    OnStrikerLevelUpEvent?.Invoke(striker_);
                    if (striker_.statsData != null)
                    {
                        striker_.statsData.OnStrikerLevelUp(newLevel - oldLevel, striker_.ID.strikerArchetype);
                    }
                }
            }
        }
        #endregion
    }
}
