using Gameplay.AI;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UIInventoryItem Parent
/// </summary>
public class DragDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("DragDrop")]

    [SerializeField] private GameObject contentParent;
    [SerializeField] private GameObject topDragLayer;
    [SerializeField] private Canvas canvas;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 startLocation;
    [SerializeField] private Vector2 center;

    public bool slotted;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private List<GameObject> slots = new List<GameObject>();

    public virtual void Awake()
    {
        center = new Vector2(0.5f, 0.5f);
        rectTransform = GetComponent<RectTransform>();
        startLocation = rectTransform.anchoredPosition;
        canvasGroup = GetComponent<CanvasGroup>();
        contentParent = GameObject.Find("StrikersContent");
        topDragLayer = GameObject.Find("TopLayer");
        canvas = GameObject.FindGameObjectWithTag("MainCanvas").GetComponent<Canvas>();
        slotManager = GameObject.FindGameObjectWithTag("SlotManager").GetComponent<SlotManager>();
        slots = slotManager.GetSlots();
        //slotted = false; // get state from server here?
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        slotted = false;
        canvasGroup.alpha= .9f;
        canvasGroup.blocksRaycasts = false;
        rectTransform.transform.SetParent(topDragLayer.transform);

    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerDrag != null)
        {
            for (int slot = 0; slot < slots.Count; slot++) // Check all slots
            {
                if (eventData.pointerCurrentRaycast.gameObject == slots[slot].gameObject) //If where dropped is one of the 3 slots
                {

                    slotted = true; // Is the card slotted?
                    rectTransform.transform.SetParent(slots[slot].gameObject.GetComponent<RectTransform>()); //Set current dragged as child from slot

                    rectTransform.pivot = center;
                    rectTransform.anchorMin = center;
                    rectTransform.anchorMax = center;
                    rectTransform.anchoredPosition = Vector2.zero;
                    slotManager.GetStrikers()[slot] = this.gameObject; //here store information in separate array to store in server?
                    //Instantiate(striker); //Spawn striker logic here? Another script top handle this? Separar server logic de client-side logic possibly
                    break;
                }
            }

            if (!slotted)
            {
                rectTransform.transform.SetParent(contentParent.transform);
                //rectTransform.anchoredPosition = startLocation; // Not necessary because Grid Layout sorts objects
                //Debug.Log("Returned to parent");

            }
        }
    }

    public virtual void SetSlotted(bool isSlotted)
    {
        slotted = isSlotted;
    }

}