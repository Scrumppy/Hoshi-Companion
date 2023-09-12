using Gameplay.Managers;
using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerDataManager : Manager<StrikerDataManager>
{
    private Dictionary<int, StrikerInfoData> strikerDataDictionary = new Dictionary<int, StrikerInfoData>();

    /// <summary>
    /// Registers a StrikerInfoData with the specified respective ID.
    /// </summary>
    /// <param name="strikerID">The ID to register.</param>
    /// <param name="strikerData">The StrikerInfoData to register.</param>
    public void RegisterData(int strikerID, StrikerInfoData strikerData)
    {
        if (!strikerDataDictionary.ContainsKey(strikerID))
        {
            strikerDataDictionary.Add(strikerID, strikerData);
        }
    }

    /// <summary>
    /// Returns a the respective StrikerInfoData from the specified ID.
    /// </summary>
    /// <param name="strikerID">The ID from the StrikerInfoData to retrieve.</param>
    public StrikerInfoData GetDataFromID(int strikerID)
    {
        if (strikerDataDictionary.TryGetValue(strikerID, out StrikerInfoData strikerData))
        {
            return strikerData;
        }

        Debug.LogWarning("The Striker ID " + strikerID + " does not exist!");
        return null;
    }
}
