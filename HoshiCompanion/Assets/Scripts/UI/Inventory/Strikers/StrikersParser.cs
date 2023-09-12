using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikersParser : MonoBehaviour
{
    private StrikersCardSO strikerdata;
    [System.Serializable]
    public class Striker
    {
        public int token_id;
        public string name;
        public string description;
        public string collection;
        public int rarity;
        public Sprite icon;
        public string image_url;
        public string animation_url;
        public int[] visualFeatures;
        public int[] paceSkillTree;
        public int[] passingSkillTree;
        public int[] shootingSkillTree;
        public int[] staminaSkillTree;
        public string archetype;
        public AttributeData[] attributes;
        public int nftId;
        public int pacePercentage;
        public int passingPercentage;
        public int shootingPercentage;
        public int staminaPercentage;
        public int pace;
        public int passing;
        public int shooting;
        public int stamina;
        public string skin;
        public string hair;
        public string beard;
        public string eyebrows;
        public string hairColour;
        public string skinColour;
        public string eyes;
        public string mouth;
        public string headwear;
        public string eyewear;
        public int lifetime;
        public int gamesPlayed;
        public int totalExpGained;
        public int level;
        public int goalsSccored;
        public string metapower;
        public int metapowerUsage;
        public int tournamentsWon;
        public int shotsOnTarget;
        public int tacklesMade;
        public int passesMade;
        public int skillPointsEarned;
        public int skillPointsSpent;
        public int nextSkillPoint;
        public int creator_fee;
        public string fee_recipient;
        [System.Serializable]
        public class AttributeData
        {
            public string trait_type;
            public string value;
            public string display_type;
            public int? max_value;
            public int[] value_array;
        }
        [System.Serializable]
        public class VisualFeatures
        {
            public int[] index;
        }
        [System.Serializable]
        public class SkillTree
        {
            public int[] nodes;
        }
        public void SetupStriker()
        {
            GetVisualFeaturesArray();
            GetPaceSkillTreeArray();
            GetPassingSkillTreeArray();
            GetShootingSkillTreeArray();
            GetStaminaSkillTreeArray();
            GetAllOtherFeatures();
        }
        public void GetVisualFeaturesArray()
        {
            foreach (AttributeData attribute in attributes)
            {
                if (attribute.trait_type == "visual_features")
                {
                    string dummy = attribute.value;
                    dummy = dummy.Replace("[", "");
                    dummy = dummy.Replace("]", "");
                    int[] valueArray = System.Array.ConvertAll(dummy.Split(','), int.Parse);
                    visualFeatures = valueArray;
                }
            }
        }
        public void GetPaceSkillTreeArray()
        {
            foreach (AttributeData attribute in attributes)
            {
                if (attribute.trait_type == "pace_skill_tree")
                {
                    string dummy = attribute.value;
                    dummy = dummy.Replace("[", "");
                    dummy = dummy.Replace("]", "");
                    int[] valueArray = System.Array.ConvertAll(dummy.Split(','), int.Parse);
                    paceSkillTree = valueArray;
                }
            }
        }
        public void GetPassingSkillTreeArray()
        {
            foreach (AttributeData attribute in attributes)
            {
                if (attribute.trait_type == "passing_skill_tree")
                {
                    string dummy = attribute.value;
                    dummy = dummy.Replace("[", "");
                    dummy = dummy.Replace("]", "");
                    int[] valueArray = System.Array.ConvertAll(dummy.Split(','), int.Parse);
                    passingSkillTree = valueArray;
                }
            }
        }
        public void GetShootingSkillTreeArray()
        {
            foreach (AttributeData attribute in attributes)
            {
                if (attribute.trait_type == "shooting_skill_tree")
                {
                    string dummy = attribute.value;
                    dummy = dummy.Replace("[", "");
                    dummy = dummy.Replace("]", "");
                    int[] valueArray = System.Array.ConvertAll(dummy.Split(','), int.Parse);
                    shootingSkillTree = valueArray;
                }
            }
        }
        public void GetStaminaSkillTreeArray()
        {
            foreach (AttributeData attribute in attributes)
            {
                if (attribute.trait_type == "stamina_skill_tree")
                {
                    string dummy = attribute.value;
                    dummy = dummy.Replace("[", "");
                    dummy = dummy.Replace("]", "");
                    int[] valueArray = System.Array.ConvertAll(dummy.Split(','), int.Parse);
                    staminaSkillTree = valueArray;
                }
            }
        }
        public void GetAllOtherFeatures()
        {
            foreach (AttributeData attribute in attributes)
            {
                switch (attribute.trait_type)
                {
                    case "striker_archetype":
                        archetype = attribute.value;
                        break;
                    case "pace_percentage":
                        pacePercentage = int.Parse(attribute.value);
                        break;
                    case "passing_percentage":
                        passingPercentage = int.Parse(attribute.value);
                        break;
                    case "shooting_percentage":
                        shootingPercentage = int.Parse(attribute.value);
                        break;
                    case "stamina_percentage":
                        staminaPercentage = int.Parse(attribute.value);
                        break;
                    case "pace":
                        pace = int.Parse(attribute.value);
                        break;
                    case "passing":
                        passing = int.Parse(attribute.value);
                        break;
                    case "shooting":
                        shooting = int.Parse(attribute.value);
                        break;
                    case "stamina":
                        stamina = int.Parse(attribute.value);
                        break;
                    case "skin":
                        skin = attribute.value;
                        break;
                    case "hair":
                        hair = attribute.value;
                        break;
                    case "beard":
                        beard = attribute.value;
                        break;
                    case "eyebrows":
                        eyebrows = attribute.value;
                        break;
                    case "hair_colour":
                        hairColour = attribute.value;
                        break;
                    case "skin_colour":
                        skinColour = attribute.value;
                        break;
                    case "eyes":
                        eyes = attribute.value;
                        break;
                    case "mouth":
                        mouth = attribute.value;
                        break;
                    case "headwear":
                        headwear = attribute.value;
                        break;
                    case "eyewear":
                        eyewear = attribute.value;
                        break;
                    case "lifetime":
                        lifetime = int.Parse(attribute.value);
                        break;
                    case "games_played":
                        gamesPlayed = int.Parse(attribute.value);
                        break;
                    case "total_exp_gained":
                        totalExpGained = int.Parse(attribute.value);
                        break;
                    case "level":
                        level = int.Parse(attribute.value);
                        break;
                    case "goals_scored":
                        goalsSccored = int.Parse(attribute.value);
                        break;
                    case "meta_power":
                        metapower = attribute.value;
                        break;
                    case "meta_power_usage":
                        metapowerUsage = int.Parse(attribute.value);
                        break;
                    case "tournaments_won":
                        tournamentsWon = int.Parse(attribute.value);
                        break;
                    case "shots_on_target":
                        shotsOnTarget = int.Parse(attribute.value);
                        break;
                    case "tackles_made":
                        tacklesMade = int.Parse(attribute.value);
                        break;
                    case "passes_made":
                        passesMade = int.Parse(attribute.value);
                        break;
                    case "skill_points_earned":
                        skillPointsEarned = int.Parse(attribute.value);
                        break;
                    case "skill_points_spent":
                        skillPointsSpent = int.Parse(attribute.value);
                        break;
                    case "next_skill_point":
                        nextSkillPoint = int.Parse(attribute.value);
                        break;
                    case "nft_id":
                        nftId = int.Parse(attribute.value);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
