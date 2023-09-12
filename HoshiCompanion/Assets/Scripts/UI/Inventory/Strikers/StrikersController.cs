using Gameplay.AI;
using Gameplay.Managers;
using System.Collections;
using System.Collections.Generic;
using UI.Character;
using UnityEngine;

/// <summary>
/// UIInventoryPage 
/// Controller Strikers
/// </summary>
public class StrikersController : MonoBehaviour
{
    [SerializeField]
    private StrikersManager strikersInventoryView;

    [SerializeField]
    private StrikersInventorySO strikersInventoryModel;

    [SerializeField]
    private GameObject strikerPrefab;

    private List<StrikerCardScript> viewCardList = new();

    public bool canUpdate = true;

    private void Start()
    {
        //First connect to DB and get the data
        strikersInventoryModel.StrikerJsonsToAssets();
        // Creates X prefabs according to inventory size
        strikersInventoryView.InitializeInventoryUI(strikersInventoryModel.inventorySize);

        // Method below serves to update the inventory
        // place this in update with a bool when inventory is showing
        foreach (var item in strikersInventoryModel.UpdateStrikersInventoryState())
        {           
            Debug.Log(item.Value.strikerData.icon);
            Debug.Log("Startyed");


            strikersInventoryView.UpdateData(item.Key, item.Value.strikerData.icon); // here pass whatever values you need using item.something
        }
        strikersInventoryView.PassObject += OnPassedObject;
    }
    private void Update()
    {
//         if (strikersInventoryView.enabled)
//         {
//             canUpdate= true;
            if (canUpdate)
            {
                canUpdate= false;
                foreach (var item in strikersInventoryModel.UpdateStrikersInventoryState())
                {

                    //Debug.Log(item.Value.striker.icon);
                    strikersInventoryView.UpdateData(item.Key, item.Value.strikerData.icon); // here pass whatever values you need using item.something

                }
                Debug.Log("Updated Once");
                //inventory shows up, update inventory
            }
        //}
        
    }
    public StrikersCardSO GetModelStrikerList(int index)
    {
        List<StrikerCardStruct> modelCardList = new();
        modelCardList = strikersInventoryModel.ReturnCardList();
        return modelCardList[index].strikerData;
    }
    private void OnPassedObject(int cardIndex, GameObject card) 
    {
        Debug.Log("The Game Object is " + card);

        StrikersCardSO cardData = GetModelStrikerList(cardIndex);

        card.GetComponent<StrikerCardLevel>()?.ReceiveStrikerCardID(cardData.token_id);

        /*GameObject strikerInstance = Instantiate(strikerPrefab, new Vector3(3,1,0)  , Quaternion.identity)*/;

        GameManager.Instance.SpawnStriker(cardData.token_id);

        //StrikerAI strikerAI = strikerInstance.GetComponent<StrikerAI>();
        //strikerAI.AssignStrikerInfoData(cardData.token_id);

        Debug.Log("The Card ID is " + cardData.token_id);

    }

}