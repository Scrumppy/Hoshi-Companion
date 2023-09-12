using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData.Strikers
{
    [CreateAssetMenu(fileName = "StrikerUpgradeData", menuName = "PlayerData/StrikerUpgradeData")]
    public class StrikerUpgradeDataAsset : ScriptableObject
    {
        public StrikerUpgradeData data;
    }

    [System.Serializable]
    public class StrikerUpgradeData
    {
        public int maxStatsPoints = 100;
        public float baseStatsValue = 1;
        public int baseStatsPoints = 0;
        public int statsPointsPerLevel = 4;
        public float statsPointValue = 1.4f;
        public Vector2 mainStatsUpgradeChance = new Vector2(30f, 33f);
        public Vector2 secondStatsUpgradeChance = new Vector2(27f, 28f);
        public Vector2 thirdStatsUpgradeChance = new Vector2(25f, 25f);
        public Vector2 fourthStatsUpgradeChance = new Vector2(14f, 18f);

        public float MinStatsValue { get => baseStatsValue + (baseStatsPoints * statsPointValue); }
        public float GetMaxStatsValue(int maxLevel) { return baseStatsValue + ((baseStatsPoints + maxLevel) * statsPointValue); }
    }
}
