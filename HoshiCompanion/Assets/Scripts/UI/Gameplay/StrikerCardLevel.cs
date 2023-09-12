using PlayerData;
using PlayerData.Strikers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Character
{
    public class StrikerCardLevel : MonoBehaviour
    {
        [Header("Card ID")]
        [SerializeField] private int cardID;

        [Header("Components")]
        public TextMeshProUGUI txtStrikerLevel;
        public Image sliderStrikerXp;
        public GameObject xpArrowObject;

        [Header("Settings")]
        public float sliderFillVelocity = 0.5f;

        private StrikerInfoData strikerInfoData;

        private void Start()
        {
            if (PlayerInfoManager.Instance != null) 
            {
                PlayerInfoManager.Instance.OnUpdateStrikerXP += OnUpdateXP;
                PlayerInfoManager.Instance.OnStrikerLevelUpEvent += OnUpdateLevel;
            }

            if (StrikerDataManager.Instance != null ) 
            {
                strikerInfoData = StrikerDataManager.Instance.GetDataFromID(cardID);
            }

            SetXPBarAndLevel(ref strikerInfoData);
        }

        private void OnEnable()
        {
            if (PlayerInfoManager.Instance != null)
            {
                PlayerInfoManager.Instance.OnUpdateStrikerXP += OnUpdateXP;
                PlayerInfoManager.Instance.OnStrikerLevelUpEvent += OnUpdateLevel;
            }

            if (StrikerDataManager.Instance != null)
            {
                strikerInfoData = StrikerDataManager.Instance.GetDataFromID(cardID);
            }

            SetXPBarAndLevel(ref strikerInfoData);
        }

        private void OnDisable()
        {
            if (PlayerInfoManager.Instance != null)
            {
                PlayerInfoManager.Instance.OnUpdateStrikerXP -= OnUpdateXP;
                PlayerInfoManager.Instance.OnStrikerLevelUpEvent -= OnUpdateLevel;
            }
        }

        private void OnUpdateXP(StrikerInfoData strikerData)
        {
            if (cardID == strikerData.ID.id)
            {
                int currentLevel = PlayerInfoManager.Instance.xpNeededToLevelUpStriker.GetLevelFromXP(strikerData.ID.currentXp);
                float percentage = PlayerInfoManager.Instance.xpNeededToLevelUpStriker.GetPercentageToLevelUp(currentLevel, strikerData.ID.currentXp);

                sliderStrikerXp.fillAmount = percentage;
            }
        }

        private void OnUpdateLevel(StrikerInfoData strikerData)
        {
            if (cardID == strikerData.ID.id)
            {
                if (txtStrikerLevel != null)
                {
                    txtStrikerLevel.text = "Lv" + strikerData.ID.level;
                    StartCoroutine(AnimateXPArrow());
                }
            }
        }

        private void SetXPBarAndLevel(ref StrikerInfoData strikerInfoData)
        {
            if (strikerInfoData == null) return;

            if (PlayerInfoManager.Instance != null)
            {
                int currentLevel = PlayerInfoManager.Instance.xpNeededToLevelUpStriker.GetLevelFromXP(strikerInfoData.ID.currentXp);
                float percentage = PlayerInfoManager.Instance.xpNeededToLevelUpStriker.GetPercentageToLevelUp(currentLevel, strikerInfoData.ID.currentXp);

                txtStrikerLevel.text = "Lv" + strikerInfoData.ID.level;
                sliderStrikerXp.fillAmount = percentage;
            }
        }

        public IEnumerator AnimateXPArrow()
        {
            xpArrowObject.SetActive(true);

            Animator animator = xpArrowObject.GetComponent<Animator>();

            if (animator)
            {
                while (true)
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

                    if (stateInfo.normalizedTime >= stateInfo.length)
                    {
                        xpArrowObject.SetActive(false);
                        yield break;
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                xpArrowObject.SetActive(false);
            }
        }

        public void ReceiveStrikerCardID(int _cardID)
        {
            cardID = _cardID;
        }
    }
}
