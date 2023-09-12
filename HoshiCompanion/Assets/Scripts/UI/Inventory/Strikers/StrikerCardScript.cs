using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UIInventoryItem Child
/// </summary>
public class StrikerCardScript : DragDrop, IPointerDownHandler, IDropHandler
{
    [Header("Components")]

    [SerializeField] private GameObject strikerInstance;

    [SerializeField] private Image strikerPortrait;

    [SerializeField] private Image strikerType;

    [SerializeField] private Image highlightedImage;

    [SerializeField] private int listPosition;

    private bool empty = true;

    public event Action<StrikerCardScript> OnItemClicked,
            OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;  
    public override void Awake()
    {
        base.Awake();
        ResetData();
        Deselect();
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (empty) return;
        OnItemBeginDrag?.Invoke(this);
    }
    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (empty) return;
        Debug.Log("1st");
        OnItemEndDrag?.Invoke(this);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (empty) return;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseBtnClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }
    public void SetCardData(Sprite sprite)
    {
        //Debug.Log(sprite);
        this.strikerPortrait.gameObject.SetActive(true);
        this.strikerPortrait.sprite = sprite;
        empty = false;

        // Add as many variables here as needed to change the card visual aspect
    }
    public void ResetData()
    {
        this.strikerPortrait.gameObject.SetActive(false);
        empty = true;
    }
    public void Deselect()
    {
        //this.highlightedImage.enabled = false;
    }
    public void SetListPosition(int position) 
    {
        listPosition = position;
    }
}