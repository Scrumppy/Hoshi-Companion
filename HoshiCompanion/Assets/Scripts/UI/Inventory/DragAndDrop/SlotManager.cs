using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour//, IDropHandler
{
    [Header("Components")]

    [SerializeField] private List<GameObject> slots = new List<GameObject>();
    // Use these lists to then save the players current strikers, to update when he closes the game and comes back
    [SerializeField] private List<GameObject> strikers = new List<GameObject>();

    [SerializeField] private GameObject striker;

    private RectTransform draggedObjectRect;

    private DragDrop dragDropScript;

    private void Awake()
    {
        for (int i = 0;i < 3; i++)
        {
            strikers.Add(null);
        }
    }

    /*public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            for (int slot = 0; slot < slots.Count; slot++) // Check all slots
            {
                if (eventData.pointerCurrentRaycast.gameObject == slots[slot].gameObject) //If where dropped is one of the 3 slots
                {
                    draggedObjectRect = eventData.pointerDrag.GetComponent<RectTransform>();// Get current dragged Rect
                    dragDropScript = draggedObjectRect.gameObject.GetComponent<DragDrop>(); //get DragDrop component

                    dragDropScript.SetSlotted(true); // Is the card slotted?
                    draggedObjectRect.transform.SetParent(slots[slot].gameObject.GetComponent<RectTransform>()); //Set current dragged as child from slot

                    draggedObjectRect.pivot = center;
                    draggedObjectRect.anchorMin = center;
                    draggedObjectRect.anchorMax = center;
                    draggedObjectRect.anchoredPosition = Vector2.zero;
                    strikers[slot] = draggedObjectRect.gameObject; //here store information in separate array to store in server?
                    Debug.Log(slots);
                    //Instantiate(striker); //Spawn striker logic here? Another script top handle this? Separar server logic de client-side logic possibly
                    break;
                }
            }
        }
    }*/

    public List<GameObject> GetSlots() 
    {
        return slots;
    }
    public List<GameObject> GetStrikers()
    {
        return strikers;
    }
}
