using PlayerData.Strikers;
using Tools;
using UnityEditor;
using UnityEngine;

public class CharacterInfo : MonoBehaviour
{
    private StrikerInfoData strikerData = new StrikerInfoData();

    [Header("Striker ID Info")]
    [ReadOnly][SerializeField] private StrikerIDInfo strikerIDInfo;

    [Header("Striker Stats Info")]
    [ReadOnly][SerializeField] private StrikerStatsInfo strikerStatsInfo;

    public StrikerInfoData Data => strikerData;

    private void Start()
    {
        //if (strikerData != null)
        //{
        //    strikerData.statsData.SetStats();
        //    strikerIDInfo = strikerData.ID;
        //    strikerStatsInfo = strikerData.statsData;
        //}
    }

    public void SetData(StrikerInfoData strikerInfoData)
    {
        strikerData = strikerInfoData;
        //strikerData.statsData.SetStats();
        strikerIDInfo = strikerData.ID;
        strikerStatsInfo = strikerData.statsData;
    }
}

