using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public struct AssetReference<T> where T : ScriptableObject
    {
        private List<T> assets;

        public List<T> Assets
        {
            get
            {
#if UNITY_EDITOR
                if (assets == null)
                    return new List<T>(AssetsManager.FindAssetsByType<T>());
#endif
                return assets;
            }

            internal set { assets = value; }
        }


        public void SetAssets(List<T> values)
        {
            assets = new List<T>(values);
        }


        //public T FindAsset(int id)
        //{
        //    List<T> a = Assets;
        //    if (a != null && a.Count > 0)
        //    {
        //        for (int i = 0, n = a.Count; i < n; i++)
        //        {
        //            T asset = a[i];
        //            if (asset.id == id)
        //                return asset;
        //        }
        //    }

        //    return null;
        //}


        //public int FindRandomId()
        //{
        //    List<T> a = Assets;
        //    if (a != null && a.Count > 0)
        //        return a[Random.Range(0, a.Count)].id;

        //    return 0;
        //}
    }

    public class AssetsManager : MonoBehaviour
    {
        [Header("Strikers Skins")]
        //public List<StrikerSkinData> strikerSkins;
        //public List<HairSkinData> hairSkins;
        //public List<HairColorData> hairColors;
        //public List<BeardSkinData> beardSkins;
        //public List<EyebrowSkinData> eyebrowSkins;
        //public List<ToneSkinData> toneSkins;
        //public List<EyeSkinData> eyeSkins;
        //public List<MouthSkinData> mouthSkins;
        //public List<HeadwearSkinData> headwearSkins;
        //public List<EyewearSkinData> eyewearSkins;

        //[Header("Strikers Skills")]
        //public List<SkillTreeNodeData> skills;

        [Header("Strikers Stats")]
        public StrikerUpgradeDataAsset strikerUpgradeData;

        //[Header("Quests")]
        //public QuestSettingsData questSettings;
        //public List<QuestContentData> quests;


        private void Awake()
        {
            Initialize();
        }


        void Initialize()
        {
            //ApplicationManager.strikerSkins.SetAssets(strikerSkins);
            //ApplicationManager.hairSkins.SetAssets(hairSkins);
            //ApplicationManager.hairColors.SetAssets(hairColors);
            //ApplicationManager.beardSkins.SetAssets(beardSkins);
            //ApplicationManager.eyebrowSkins.SetAssets(eyebrowSkins);
            //ApplicationManager.toneSkins.SetAssets(toneSkins);
            //ApplicationManager.eyeSkins.SetAssets(eyeSkins);
            //ApplicationManager.mouthSkins.SetAssets(mouthSkins);
            //ApplicationManager.headwearSkins.SetAssets(headwearSkins);
            //ApplicationManager.eyewearSkins.SetAssets(eyewearSkins);

            //ApplicationManager.skillTreeNodes.SetAssets(skills);

            StrikerStatsInfo.SetUpgradeData(strikerUpgradeData);

            //QuestManager.SetSettings(questSettings);
            //QuestManager.SetQuests(quests);
        }


#if UNITY_EDITOR
        internal static IEnumerable<T> FindAssetsByType<T>() where T : Object
        {
            var guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var t in guids)
            {
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(t);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    yield return asset;
                }
            }
        }
#endif
    }
}
