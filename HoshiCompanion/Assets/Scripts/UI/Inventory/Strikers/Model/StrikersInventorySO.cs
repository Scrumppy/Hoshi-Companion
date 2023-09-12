using PlayerData.Strikers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// InventorySO
/// Modal Strikers
/// </summary>
[CreateAssetMenu] // This script is responsible for holding all the strikers data, the Modal of the MVC
public class StrikersInventorySO : ScriptableObject
{
    [field: SerializeField] private List<StrikerCardStruct> strikersStructList;

    //Get number of NFT cards in player wallet
    [field: SerializeField] public int inventorySize { get; private set; } = 0;

    // Value needs to be the last id that exists in database
    [field: SerializeField] public int MaxIdValue { get; private set; } = 9999; 

    [field: SerializeField] private string _pathToFolder;
    private string pathToFolder;

    [field: SerializeField] private string _pathToSave;
    private string pathToSave;

    private string pathToFile;

    private string jsonContent;

    private string fileName;

    private string assetPath;

    private void Awake()
    {
        pathToFolder = _pathToFolder;
        pathToSave = _pathToSave;
    }

    private void OnEnable()
    {
        strikersStructList.Clear();
        strikersStructList.Capacity = 0;
        strikersStructList = new();
    }
    public void Initialize()
    {
        strikersStructList = new List<StrikerCardStruct>();

        for (int i = 0; i < inventorySize; i++)
        {
            strikersStructList.Add(StrikerCardStruct.GetEmptyItem());
        }
    }

    public void AddItem(StrikersCardSO striker)
    {
        for (int i = 0; i < strikersStructList.Count; i++)
        {
            if (strikersStructList[i].IsEmpty)
            {
                strikersStructList[i] = new()
                {
                    strikerData = striker
                };
            }
        }
    }

    public void StrikerJsonsToAssets()
    {
        inventorySize = 0;
        for (int i = 0; i < MaxIdValue; i++)
        {
            pathToFile = Path.Combine(pathToFolder, i + ".json");

            if (File.Exists(pathToFile))
            {
                inventorySize += 1;
                jsonContent = File.ReadAllText(pathToFile);
                //Debug.Log(pathToFile);

                CreateStrikerData(jsonContent, (strikerCardSO) =>
                {
                    StrikerCardStruct newStrikerStruct = new()
                    {
                        strikerData = strikerCardSO
                    };

                    strikersStructList.Add(newStrikerStruct);

                    fileName = strikerCardSO.token_id.ToString();
                    assetPath = Path.Combine(pathToSave, fileName + ".asset");

                    AssetDatabase.CreateAsset(strikerCardSO, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    //Debug.Log("StrikersCardSO " + strikerCardSO + "created and saved as asset.");
                    //Debug.Log(newStrikerCard.striker.attributes);
                });
            }
            else
            {
                continue;
            }
        }
    }

    public void CreateStrikerData(string jsonFileData, Action<StrikersCardSO> callback)
    {
        StrikersParser.Striker striker = JsonUtility.FromJson<StrikersParser.Striker>(jsonFileData);
        StrikersCardSO strikerData = ScriptableObject.CreateInstance<StrikersCardSO>();

        // Load image from URL only if no existing icon is present
        UnityWebRequest requestIcon = UnityWebRequestTexture.GetTexture(striker.image_url);
        UnityWebRequestAsyncOperation asyncOperation = requestIcon.SendWebRequest();
        
        asyncOperation.completed += (operation) =>
        {
            if (requestIcon.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load image from URL: " + striker.image_url);
                return;
            }
        
            Texture2D texture = DownloadHandlerTexture.GetContent(requestIcon);
            Sprite loadedSprite = SpriteFromTexture(texture);
        
            // Set the icon with the loaded sprite
            striker.icon = loadedSprite;
        
            // Set data in the StrikerCardSO using the parsed values
            strikerData.SetData(
                striker.icon,
                striker.token_id,
                striker.name,
                striker.description,
                striker.collection,
                striker.rarity,
                striker.image_url,
                striker.animation_url,
                ParseAttributes(striker.attributes)
            );
        
            striker.SetupStriker();
        
            // Call the callback with the created StrikersCardSO
            callback?.Invoke(strikerData);
        };

        StrikerInfoData strikerInfoData = CreateStrikerInfoData(striker);
        StrikerDataManager.Instance.RegisterData(striker.token_id, strikerInfoData);
    }

    /// <summary>
    /// This function is responsible for creating a new striker data ScriptableObject asset
    /// we can then use this data to create a gameobject of a striker and give him this data
    /// </summary>

    private Sprite SpriteFromTexture(Texture2D dwnld_Texture)
    {
        Rect rect = new Rect(0, 0, dwnld_Texture.width, dwnld_Texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        Sprite cardIcon = Sprite.Create(dwnld_Texture, rect, pivot);
        return cardIcon;
    }

    /// <summary>
    /// This function is responsible for parsing the striker attributes. 
    /// It uses the list AttributeData inside the Striker Parser script / Striker class within.
    /// attributeData is populated using by using the parsed data from the json file inside the CreateStrikersCardSO() function.
    /// Returns an attribute list.
    /// </summary>
    private List<Attribute> ParseAttributes(StrikersParser.Striker.AttributeData[] attributeData)
    {
        List<Attribute> attributes = new();

        foreach (StrikersParser.Striker.AttributeData attribute in attributeData)
        {
            Attribute newAttribute = new()
            {
                trait_type = attribute.trait_type,
                value = attribute.value

            };
            
            //Debug.Log(attribute.trait_type);
            //Debug.Log(attribute.value);


            attributes.Add(newAttribute);
        }
        return attributes;
    }

    public StrikerInfoData CreateStrikerInfoData(StrikersParser.Striker striker)
    {
        StrikerInfoData data = new StrikerInfoData();

        data.ID = new StrikerIDInfo()
        {
            id = striker.token_id,
            strikerName = striker.name,
            level = striker.level,
            currentXp = striker.totalExpGained,
        };

        switch (striker.archetype)
        {
            case "Finisher": data.ID.strikerArchetype = StrikerArchetype.Finisher; break;
            case "Speedster": data.ID.strikerArchetype = StrikerArchetype.Speedster; break;
            case "Maestro": data.ID.strikerArchetype = StrikerArchetype.Maestro; break;
            case "Engine": data.ID.strikerArchetype = StrikerArchetype.Engine; break;
        }

        //// Setup Skin
        //data.skinData = new StrikerSkinInfo()
        //{
        //    baseSkinId = striker.visualFeatures[0],     // 0
        //    hairId = striker.visualFeatures[1],         // 1
        //    beardId = striker.visualFeatures[2],        // 2
        //    eyesId = striker.visualFeatures[3],         // 3
        //    skinColorId = striker.visualFeatures[4],    // 4
        //    mouthId = striker.visualFeatures[5],        // 5
        //    hairColourId = striker.visualFeatures[6],   // 6
        //    themeColourId = striker.visualFeatures[7],  // 7
        //    eyebrowsId = striker.visualFeatures[8],     // 8
        //    headwearId = striker.visualFeatures[9],     // 9
        //    eyewearId = striker.visualFeatures[10],      // 10
        //};

        //// Manual Fixing skin
        //if (striker.skin == "Punkerino")
        //    data.skinData.baseSkinId = 6;

        // Setup stats
        int baseStatsPoints = StrikerStatsInfo.UpgradeData.baseStatsPoints;
        data.statsData = new StrikerStatsInfo()
        {
            //Subtracting baseStatsPoints because due to offline default settings
            shootingPoints = striker.shooting - baseStatsPoints,
            pacePoints = striker.pace - baseStatsPoints,
            passingPoints = striker.passing - baseStatsPoints,
            staminaPoints = striker.stamina - baseStatsPoints,
        };
        foreach (StrikerStatsType statsType in Enum.GetValues(typeof(StrikerStatsType)))
        {
            switch (statsType)
            {
                case StrikerStatsType.Shooting: data.statsData.SetArchetypeStatsUpgradeChance(data.ID.strikerArchetype, statsType, striker.shootingPercentage); break;
                case StrikerStatsType.Pace: data.statsData.SetArchetypeStatsUpgradeChance(data.ID.strikerArchetype, statsType, striker.pacePercentage); break;
                case StrikerStatsType.Passing: data.statsData.SetArchetypeStatsUpgradeChance(data.ID.strikerArchetype, statsType, striker.passingPercentage); break;
                case StrikerStatsType.Stamina: data.statsData.SetArchetypeStatsUpgradeChance(data.ID.strikerArchetype, statsType, striker.staminaPercentage); break;
            }
        }
        data.statsData.SetStats();

        //Debug.Log("Striker ID: " + data.ID.id + "; Name: " + data.ID.strikerName + "; Level: " + data.ID.level + "; Archetype: " + data.ID.strikerArchetype);

        return data;
    }

    [SerializeField]
    public Dictionary<int, StrikerCardStruct> UpdateStrikersInventoryState()
    {
        //Creates a dictionary for each strikerData, [ striker index ]
        Dictionary<int, StrikerCardStruct> returnValue = new();

        for (int i = 0; i < strikersStructList.Count; i++)
        {
            if(strikersStructList[i].IsEmpty) 
                continue;
            returnValue[i] = strikersStructList[i];
        }
        //each return value is printed as, for example striker 1, [index 0, StrikerCard struct]
        return returnValue;
    }

    public List<StrikerCardStruct> ReturnCardList()
    {
        return strikersStructList;
    }

}

[System.Serializable]
public struct StrikerCardStruct
{
    [SerializeField] public StrikersCardSO strikerData;
    
    public bool IsEmpty => strikerData == null;
    public static StrikerCardStruct GetEmptyItem() => new()
    {
        strikerData = null
    };
}

