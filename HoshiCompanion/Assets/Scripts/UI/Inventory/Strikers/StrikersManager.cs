using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UIInventoryPage
/// View Strikers
/// </summary>
public class StrikersManager : MonoBehaviour
{
    [SerializeField] private StrikerCardScript cardPrefab;

    [SerializeField] private RectTransform contentPanel;

    [SerializeField] public List<StrikerCardScript> strikersInventory = new();

    public event Action<int, GameObject> PassObject;

    private int currentlyDraggedItemIndex = -1;
    public void InitializeInventoryUI(int inventorySize) 
    {
        for (int i = 0; i < inventorySize; i++)
        {
            StrikerCardScript uiCard = Instantiate(cardPrefab);
            uiCard.name = uiCard.gameObject.name + " " + i;
            uiCard.transform.SetParent(contentPanel);
            uiCard.transform.localScale = Vector3.one;
            strikersInventory.Add(uiCard);
            uiCard.OnItemClicked += HandleItemSelection;
            uiCard.OnItemBeginDrag += HandleBeginDrag;
            uiCard.OnItemDroppedOn += HandleSwap;
            uiCard.OnItemEndDrag += HandleEndDrag;
            uiCard.OnRightMouseBtnClick += HandleShowItemActions;
            uiCard.SetListPosition(i);
        }
    }
    // Down from here is where we handle actions inside our card ( this case, StrikerCardScript )
    // For example, HandleItemSelection calls a function inside our card called OnPointerDown
    private void HandleItemSelection(StrikerCardScript inventoryItemUI)
    {

    }
    private void HandleBeginDrag(StrikerCardScript inventoryItemUI)
    {
        int index = strikersInventory.IndexOf(inventoryItemUI);
        if (index == -1) 
            return;
        currentlyDraggedItemIndex = index;
    }
    private void HandleSwap(StrikerCardScript inventoryItemUI)
    {

    }
    private void HandleEndDrag(StrikerCardScript inventoryItemUI)
    {
        Debug.Log(strikersInventory.IndexOf(inventoryItemUI));
        Debug.Log(inventoryItemUI.gameObject);
        Debug.Log("2st");
        if (inventoryItemUI.slotted)
        {
            PassObject?.Invoke(strikersInventory.IndexOf(inventoryItemUI), inventoryItemUI.gameObject);

        }
    }
    private void HandleShowItemActions(StrikerCardScript inventoryItemUI)
    {

    }   
    public void UpdateData(int itemIndex, Sprite icon)
    {
        if (strikersInventory.Count > itemIndex)
        {
            strikersInventory[itemIndex].SetCardData(icon);
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
