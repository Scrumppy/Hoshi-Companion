using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    public enum XpType { PlayerXP, StrikerXP }

    [CreateAssetMenu(menuName = "PlayerData/XpNeededToLevelUpData")]
    public class XpNeededToLevelUpData : ScriptableObject
    {
        public XpType typeOfXP;

        [Header("Temporaly define a max xp to be reached, if greater than 0.\nThis is used for hold the strikers level till the striker's merge system being ready.")]
        public int tempMaxXPIndex = -1;

        [Header("The index represents the level, and the value inserted represents the xp needed to reach that level")]
        public List<float> xpNecessaryForLevel;

        public int MaxLevel { get => Mathf.Max(1, xpNecessaryForLevel.Count - 1); }

        public float MaxXP
        {
            get
            {
                if (xpNecessaryForLevel.Count == 0) return 0;
                else
                {
                    if (tempMaxXPIndex >= 0 && xpNecessaryForLevel.Count > tempMaxXPIndex)
                    {
                        return xpNecessaryForLevel[tempMaxXPIndex];
                    }

                    return Mathf.Max(xpNecessaryForLevel.ToArray());
                }
            }
        }

        //Receive the xp and return the level
        public int GetLevelFromXP(float xp)
        {
            int left = 0;
            int right = xpNecessaryForLevel.Count - 1;
            int iterationAmount = 0;

            while (left <= right)
            {
                iterationAmount++;

                int middle = (left + right) / 2;

                float xpRequired = xpNecessaryForLevel[middle];

                if (xp < xpRequired)
                {
                    right = middle - 1;
                }
                else if (xp > xpRequired)
                {
                    left = middle + 1;
                }
                else
                {
                    //Debug.Log("Level is " + middle + "nO of " + iterationAmount);
                    return middle;
                }
            }

            //Debug.Log("Level is " + right + "nO of " + iterationAmount);
            return right;
        }

        //Returns a value from 0 to 1, giving the percentage to level up
        public float GetPercentageToLevelUp(int level, float xp)
        {
            if (level < 0 || level >= xpNecessaryForLevel.Count || level + 1 >= xpNecessaryForLevel.Count)
            {
                return 1;
            }

            float percentage = (xp - xpNecessaryForLevel[level]) / (xpNecessaryForLevel[level + 1] - xpNecessaryForLevel[level]);
            percentage = Mathf.Clamp01(percentage);
            
            return percentage;
        }

        //Return the xp of the next level
        public float GetXpOfNextLevel(int level)
        {
            if (level < 0 || level >= xpNecessaryForLevel.Count || level + 1 >= xpNecessaryForLevel.Count)
            {
                return 9999;
            }

            return xpNecessaryForLevel[level + 1];
        }
    }
}

