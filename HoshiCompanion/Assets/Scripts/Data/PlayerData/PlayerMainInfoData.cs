using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerData
{
    [CreateAssetMenu(menuName = "PlayerData/PlayerMainInfoData")]
    public class PlayerMainInfoData : ScriptableObject
    {
        [Header("Info")]
        public int dataVersion;

        [Header("Player data")]
        public int bl;
        //public int tickets;
        //public float playerXp;
        //public int playerLevel;

        //public List<StrikerInfoData> nftStrikers = new List<StrikerInfoData>();
        //public List<PlayerStrikerInfoData> localStrikers = new List<PlayerStrikerInfoData>();

        //public List<StrikerInfoData> AllStrikersOfPlayer // all strikers pulled from DB or NFTS
        //{
        //    get
        //    {
        //        List<StrikerInfoData> strikers = new List<StrikerInfoData>();
        //        if (localStrikers != null && localStrikers.Count > 0)
        //            localStrikers.ForEach(x => strikers.Add(x.infoData));

        //        if (nftStrikers != null && nftStrikers.Contains(null))
        //            nftStrikers.RemoveAll(striker => striker == null);

        //        strikers.AddRange(nftStrikers);
        //        //strikers.RemoveAll(o => o == null);

        //        return strikers;
        //    }
        //}
    }
}
