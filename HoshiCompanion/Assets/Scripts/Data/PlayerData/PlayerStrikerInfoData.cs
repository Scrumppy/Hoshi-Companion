using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace PlayerData.Strikers
{
    [CreateAssetMenu(menuName = "PlayerData/PlayerStrikerInfoData")]
    public class PlayerStrikerInfoData : ScriptableObject
    {
        [Header("New Data")]
        public StrikerInfoData infoData = new StrikerInfoData();
    }

    public enum StrikerArchetype { Finisher, Speedster, Maestro, Engine }

    public enum ArchetypeStatsType { Main, Second, Third, Fourth }

    public enum StrikerStatsType { Shooting, Pace, Passing, Stamina }

    [System.Serializable]
    public class StrikerInfoData
    {
        public StrikerIDInfo ID = new StrikerIDInfo();
        public StrikerSkinInfo skinData = new StrikerSkinInfo();
        public StrikerStatsInfo statsData = new StrikerStatsInfo();

        public int Level
        {
            get => ID.level;
            set
            {
                //if (level < value)
                //    statsData.OnStrikerLevelUp(value - level, strikerArchetype);
                ID.level = value;
            }
        }
    }

    [System.Serializable]
    public class StrikerIDInfo
    {
        public int id; //token id
        public string strikerName;
        public int level;
        public float currentXp;
        public StrikerArchetype strikerArchetype;
        public string spriteURL; // sprite used for cards
        private Sprite sprite;

        public Sprite GetSprite()
        {

            return sprite;

        }

        public void SetSprite(Sprite sprite)
        {

            this.sprite = sprite;

        }

        public async Task<Sprite> GetSpriteAsync()
        {
            string textPath = Application.persistentDataPath + "/" + id + "/render.png";
            byte[] bytes;
            Texture2D render = new Texture2D(2, 2);

            if (File.Exists(textPath))
            {
                bytes = await File.ReadAllBytesAsync(textPath);

                if (!Application.isPlaying)
                    return null;

                render.LoadImage(bytes);
            }

            Sprite sprite = Sprite.Create(render, new Rect(0, 0, render.width, render.height), UnityEngine.Vector2.zero);
            return sprite;
        }
    }

    [System.Serializable]
    public class StrikerSkinInfo
    {
        public int baseSkinId;     // 0
        public int hairId;         // 1
        public int beardId;        // 2
        public int eyesId;         // 3
        public int skinColorId;    // 4
        public int mouthId;        // 5
        public int hairColourId;   // 6
        public int themeColourId;  // 7
        public int eyebrowsId;     // 8
        public int headwearId;     // 9
        public int eyewearId;      // 10
    }

    [System.Serializable]
    public class StrikerStatsInfo
    {
        private static StrikerUpgradeData upgradeData;

        public static StrikerUpgradeData UpgradeData
        {
            get
            {
#if UNITY_EDITOR
                if (upgradeData == null)
                {
                    List<StrikerUpgradeDataAsset> settingsDatas = new List<StrikerUpgradeDataAsset>(AssetsManager.FindAssetsByType<StrikerUpgradeDataAsset>());
                    if (settingsDatas != null && settingsDatas.Count > 0)
                    {
                        Debug.Log("Found UpgradeData");
                        upgradeData = settingsDatas[0].data;
                    }
                    else
                    {
                        Debug.LogWarning("Could not find UpgradeData");
                    }
                }
#endif

                return upgradeData;
            }
        }
        [Space]
        public int shootingPoints = 1;
        public int pacePoints = 1;
        public int passingPoints = 1;
        public int staminaPoints = 1;
        [Space]
        public float mainStatsUpgradeChance = -1;
        public float secondStatsUpgradeChance = -1;
        public float thirdStatsUpgradeChance = -1;
        public float fourthStatsUpgradeChance = -1;
        [Space]
        [Header("(Overwritten by each stats points)")]
        [Space]
        public float shooting = 1f;
        public float pace = 1f;
        public float passing = 1f;
        public float stamina = 1f;

        public StrikerStatsInfo() { }
        public StrikerStatsInfo(StrikerStatsInfo data)
        {
            if (data != null)
            {
                shootingPoints = data.shootingPoints;
                pacePoints = data.pacePoints;
                passingPoints = data.passingPoints;
                staminaPoints = data.staminaPoints;
                shooting = data.shooting;
                pace = data.pace;
                passing = data.passing;
                stamina = data.stamina;
            }
        }

        #region Data Set
        public static void SetUpgradeData(StrikerUpgradeDataAsset upgradeDataAsset)
        {
            upgradeData = upgradeDataAsset.data;
        }

        public void SetArchetypeStatsUpgradeChance(StrikerArchetype archetype, StrikerStatsType statsType, float upgradeChance)
        {
            switch (GetArchetypeStatsType(archetype, statsType))
            {
                default:
                case ArchetypeStatsType.Main: mainStatsUpgradeChance = upgradeChance; break;
                case ArchetypeStatsType.Second: secondStatsUpgradeChance = upgradeChance; break;
                case ArchetypeStatsType.Third: thirdStatsUpgradeChance = upgradeChance; break;
                case ArchetypeStatsType.Fourth: fourthStatsUpgradeChance = upgradeChance; break;
            }
        }
        #endregion

        #region StatsLerpValues
        public Dictionary<StrikerStatsType, float> GetStatsLerpValues(int maxLevel)
        {
            float maxValue = UpgradeData.GetMaxStatsValue(maxLevel);
            Dictionary<StrikerStatsType, float> statsLerpValues = new Dictionary<StrikerStatsType, float>();
            foreach (StrikerStatsType type in Enum.GetValues(typeof(StrikerStatsType)))
            {
                float value = 0;
                switch (type)
                {
                    case StrikerStatsType.Shooting: value = shooting / maxValue; break;
                    case StrikerStatsType.Pace: value = pace / maxValue; break;
                    case StrikerStatsType.Passing: value = passing / maxValue; break;
                    case StrikerStatsType.Stamina: value = stamina / maxValue; break;
                }
                statsLerpValues.Add(type, value);
            }
            return statsLerpValues;
        }
        #endregion /StatsLerpValues

        public int StatsPoints(int level) { return level * UpgradeData.statsPointsPerLevel; }

        public void OnStrikerLevelUp(int increasedLevels, StrikerArchetype archetype)
        {
            UpgradeRandomStats(StatsPoints(increasedLevels), archetype);
        }

        public void SetStats()
        {
            CheckStatsPointsMaxValue();
            float baseStatsValue = UpgradeData.baseStatsValue;
            float pointsValue = UpgradeData.statsPointValue;
            shooting = baseStatsValue + shootingPoints * pointsValue;
            pace = baseStatsValue + pacePoints * pointsValue;
            passing = baseStatsValue + passingPoints * pointsValue;
            stamina = baseStatsValue + staminaPoints * pointsValue;
        }

        public void RerollStats(int level, StrikerArchetype archetype)
        {
            ResetStats();
            UpgradeRandomStats(StatsPoints(level), archetype);
        }

        private void ResetStats()
        {
            int maxStatsPoints = UpgradeData.maxStatsPoints;
            int defaultValue = UpgradeData.baseStatsPoints;
            shootingPoints = Mathf.Min(defaultValue, maxStatsPoints);
            pacePoints = Mathf.Min(defaultValue, maxStatsPoints);
            passingPoints = Mathf.Min(defaultValue, maxStatsPoints);
            staminaPoints = Mathf.Min(defaultValue, maxStatsPoints);
        }

        private void CheckStatsPointsMaxValue()
        {
            int maxStatsPoints = UpgradeData.maxStatsPoints;
            shootingPoints = Mathf.Min(shootingPoints, maxStatsPoints);
            pacePoints = Mathf.Min(pacePoints, maxStatsPoints);
            passingPoints = Mathf.Min(passingPoints, maxStatsPoints);
            staminaPoints = Mathf.Min(staminaPoints, maxStatsPoints);
        }

        private void UpgradeRandomStats(int upgradePoints, StrikerArchetype archetype)
        {
            List<ArchetypeStatsType> statsToUpgrade = GetRandomArchetypeStatsToUpgrade(upgradePoints);
            CheckStatsToUpgrade(archetype, ref statsToUpgrade);
            switch (archetype)
            {
                case StrikerArchetype.Finisher: UpgradeFinisherStats(statsToUpgrade); break;
                case StrikerArchetype.Speedster: UpgradeSpeedsterStats(statsToUpgrade); break;
                case StrikerArchetype.Maestro: UpgradeMaestroStats(statsToUpgrade); break;
                case StrikerArchetype.Engine: UpgradeEngineStats(statsToUpgrade); break;
            }
            SetStats();
        }

        private void UpgradeFinisherStats(List<ArchetypeStatsType> statsToUpgrade)
        {
            statsToUpgrade.ForEach(x =>
            {
                switch (x)
                {
                    default:
                    case ArchetypeStatsType.Main: shootingPoints++; break;
                    case ArchetypeStatsType.Second: pacePoints++; break;
                    case ArchetypeStatsType.Third: staminaPoints++; break;
                    case ArchetypeStatsType.Fourth: passingPoints++; break;
                }
            });
        }

        private void UpgradeSpeedsterStats(List<ArchetypeStatsType> statsToUpgrade)
        {
            statsToUpgrade.ForEach(x =>
            {
                switch (x)
                {
                    default:
                    case ArchetypeStatsType.Main: pacePoints++; break;
                    case ArchetypeStatsType.Second: passingPoints++; break;
                    case ArchetypeStatsType.Third: staminaPoints++; break;
                    case ArchetypeStatsType.Fourth: shootingPoints++; break;
                }
            });
        }

        private void UpgradeMaestroStats(List<ArchetypeStatsType> statsToUpgrade)
        {
            statsToUpgrade.ForEach(x =>
            {
                switch (x)
                {
                    default:
                    case ArchetypeStatsType.Main: passingPoints++; break;
                    case ArchetypeStatsType.Second: pacePoints++; break;
                    case ArchetypeStatsType.Third: staminaPoints++; break;
                    case ArchetypeStatsType.Fourth: shootingPoints++; break;
                }
            });
        }

        private void UpgradeEngineStats(List<ArchetypeStatsType> statsToUpgrade)
        {
            statsToUpgrade.ForEach(x =>
            {
                switch (x)
                {
                    default:
                    case ArchetypeStatsType.Main: staminaPoints++; break;
                    case ArchetypeStatsType.Second: shootingPoints++; break;
                    case ArchetypeStatsType.Third: passingPoints++; break;
                    case ArchetypeStatsType.Fourth: pacePoints++; break;
                }
            });
        }

        private List<ArchetypeStatsType> GetRandomArchetypeStatsToUpgrade(int statsPoints)
        {
            List<ArchetypeStatsType> statsType = new List<ArchetypeStatsType>();
            while (statsType.Count < statsPoints)
            {
                float mainChance = GetMainStatsUpgradeChance(), secondChance = GetSecondStatsUpgradeChance(),
                    thirdChance = GetThirdStatsUpgradeChance(), fourthChance = GetFourthStatsUpgradeChance();

                if (UnityEngine.Random.Range(0f, 100f) < mainChance)
                    statsType.Add(ArchetypeStatsType.Main);
                else if (UnityEngine.Random.Range(0f, 100f) < secondChance)
                    statsType.Add(ArchetypeStatsType.Second);
                else if (UnityEngine.Random.Range(0f, 100f) < thirdChance)
                    statsType.Add(ArchetypeStatsType.Third);
                else if (UnityEngine.Random.Range(0f, 100f) < fourthChance)
                    statsType.Add(ArchetypeStatsType.Fourth);

                //break;
            }

            if (statsType.Count > statsPoints)
            {
                statsType.Sort((a, b) => { return a >= b ? 1 : -1; });
                statsType.RemoveAt(0);
            }

            while (statsType.Count > statsPoints)
                statsType.RemoveAt(0);

            return statsType;
        }

        private float GetMainStatsUpgradeChance()
        {
            Vector2 chanceRange = UpgradeData.mainStatsUpgradeChance;
            float x = Mathf.Min(chanceRange.x, chanceRange.y), y = Mathf.Max(chanceRange.x, chanceRange.y);
            //Debug.Log(mainStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : mainStatsUpgradeChance);
            return mainStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : mainStatsUpgradeChance;
        }

        private float GetSecondStatsUpgradeChance()
        {
            Vector2 chanceRange = UpgradeData.secondStatsUpgradeChance;
            float x = Mathf.Min(chanceRange.x, chanceRange.y), y = Mathf.Max(chanceRange.x, chanceRange.y);
            //Debug.Log(mainStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : mainStatsUpgradeChance);
            return secondStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : secondStatsUpgradeChance;
        }

        private float GetThirdStatsUpgradeChance()
        {
            Vector2 chanceRange = UpgradeData.thirdStatsUpgradeChance;
            float x = Mathf.Min(chanceRange.x, chanceRange.y), y = Mathf.Max(chanceRange.x, chanceRange.y);
            //Debug.Log(mainStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : mainStatsUpgradeChance);
            return thirdStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : thirdStatsUpgradeChance;
        }

        private float GetFourthStatsUpgradeChance()
        {
            Vector2 chanceRange = UpgradeData.fourthStatsUpgradeChance;
            float x = Mathf.Min(chanceRange.x, chanceRange.y), y = Mathf.Max(chanceRange.x, chanceRange.y);
            //Debug.Log(mainStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : mainStatsUpgradeChance);
            return fourthStatsUpgradeChance <= 0 ? UnityEngine.Random.Range(x, y) : fourthStatsUpgradeChance;
        }

        private void CheckStatsToUpgrade(StrikerArchetype archetype, ref List<ArchetypeStatsType> statsToUpgrade)
        {
            for (int i = 0; i < statsToUpgrade.Count; i++)
            {
                ArchetypeStatsType stats = statsToUpgrade[i];
                if (IsStatsMaximized(archetype, stats))
                {
                    int[] enumArray = (int[])Enum.GetValues(typeof(ArchetypeStatsType));
                    List<int> enumList = new List<int>(enumArray);
                    enumList.Remove(stats.GetHashCode());
                    stats = (ArchetypeStatsType)UnityEngine.Random.Range(Mathf.Min(enumList.ToArray()), Mathf.Max(enumList.ToArray()));
                }
                statsToUpgrade[i] = stats;
            }
        }

        private bool IsStatsMaximized(StrikerArchetype archetype, ArchetypeStatsType archetypeStats)
        {
            int maxStatsPoints = UpgradeData.maxStatsPoints;
            switch (archetype)
            {
                default:
                case StrikerArchetype.Finisher: return GetSrikerStatsPointValue(GetFinisherStrikerStatsType(archetypeStats)) >= maxStatsPoints;
                case StrikerArchetype.Speedster: return GetSrikerStatsPointValue(GetSpeedsterStrikerStatsType(archetypeStats)) >= maxStatsPoints;
                case StrikerArchetype.Maestro: return GetSrikerStatsPointValue(GetMaestroStrikerStatsType(archetypeStats)) >= maxStatsPoints;
                case StrikerArchetype.Engine: return GetSrikerStatsPointValue(GetEngineStrikerStatsType(archetypeStats)) >= maxStatsPoints;
            }
        }

        private int GetSrikerStatsPointValue(StrikerStatsType statsType)
        {
            switch (statsType)
            {
                default: return 0;
                case StrikerStatsType.Shooting: return shootingPoints;
                case StrikerStatsType.Pace: return pacePoints;
                case StrikerStatsType.Passing: return passingPoints;
                case StrikerStatsType.Stamina: return staminaPoints;
            }
        }

        private ArchetypeStatsType GetArchetypeStatsType(StrikerArchetype archetype, StrikerStatsType statsType)
        {
            switch (archetype)
            {
                default:
                case StrikerArchetype.Finisher: return GetFinisherArchetypeStatsType(statsType);
                case StrikerArchetype.Speedster: return GetSpeedsterArchetypeStatsType(statsType);
                case StrikerArchetype.Maestro: return GetMaestroArchetypeStatsType(statsType);
                case StrikerArchetype.Engine: return GetEngineArchetypeStatsType(statsType);
            }
        }

        private ArchetypeStatsType GetFinisherArchetypeStatsType(StrikerStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case StrikerStatsType.Shooting: return ArchetypeStatsType.Main;
                case StrikerStatsType.Pace: return ArchetypeStatsType.Second;
                case StrikerStatsType.Stamina: return ArchetypeStatsType.Third;
                case StrikerStatsType.Passing: return ArchetypeStatsType.Fourth;
            }
        }

        private ArchetypeStatsType GetSpeedsterArchetypeStatsType(StrikerStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case StrikerStatsType.Pace: return ArchetypeStatsType.Main;
                case StrikerStatsType.Passing: return ArchetypeStatsType.Second;
                case StrikerStatsType.Stamina: return ArchetypeStatsType.Third;
                case StrikerStatsType.Shooting: return ArchetypeStatsType.Fourth;
            }
        }

        private ArchetypeStatsType GetMaestroArchetypeStatsType(StrikerStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case StrikerStatsType.Passing: return ArchetypeStatsType.Main;
                case StrikerStatsType.Pace: return ArchetypeStatsType.Second;
                case StrikerStatsType.Stamina: return ArchetypeStatsType.Third;
                case StrikerStatsType.Shooting: return ArchetypeStatsType.Fourth;
            }
        }

        private ArchetypeStatsType GetEngineArchetypeStatsType(StrikerStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case StrikerStatsType.Stamina: return ArchetypeStatsType.Main;
                case StrikerStatsType.Shooting: return ArchetypeStatsType.Second;
                case StrikerStatsType.Passing: return ArchetypeStatsType.Third;
                case StrikerStatsType.Pace: return ArchetypeStatsType.Fourth;
            }
        }

        private StrikerStatsType GetFinisherStrikerStatsType(ArchetypeStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case ArchetypeStatsType.Main: return StrikerStatsType.Shooting;
                case ArchetypeStatsType.Second: return StrikerStatsType.Pace;
                case ArchetypeStatsType.Third: return StrikerStatsType.Stamina;
                case ArchetypeStatsType.Fourth: return StrikerStatsType.Passing;
            }
        }

        private StrikerStatsType GetSpeedsterStrikerStatsType(ArchetypeStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case ArchetypeStatsType.Main: return StrikerStatsType.Pace;
                case ArchetypeStatsType.Second: return StrikerStatsType.Passing;
                case ArchetypeStatsType.Third: return StrikerStatsType.Stamina;
                case ArchetypeStatsType.Fourth: return StrikerStatsType.Shooting;
            }
        }

        private StrikerStatsType GetMaestroStrikerStatsType(ArchetypeStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case ArchetypeStatsType.Main: return StrikerStatsType.Passing;
                case ArchetypeStatsType.Second: return StrikerStatsType.Pace;
                case ArchetypeStatsType.Third: return StrikerStatsType.Stamina;
                case ArchetypeStatsType.Fourth: return StrikerStatsType.Shooting;
            }
        }

        private StrikerStatsType GetEngineStrikerStatsType(ArchetypeStatsType statsType)
        {
            switch (statsType)
            {
                default:
                case ArchetypeStatsType.Main: return StrikerStatsType.Stamina;
                case ArchetypeStatsType.Second: return StrikerStatsType.Shooting;
                case ArchetypeStatsType.Third: return StrikerStatsType.Passing;
                case ArchetypeStatsType.Fourth: return StrikerStatsType.Pace;
            }
        }
    }
}