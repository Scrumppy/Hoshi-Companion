using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hub : MonoBehaviour
{
    [Header("General Variables")]
    [SerializeField] private GameObject parentUI;

    [Header("Buttons & Tabs")]
    [SerializeField] private Button btnHub;
    [SerializeField] private Button btnStrikersTab;
    [SerializeField] private Button btnToysTab;
    [SerializeField] private Button btnItemsTab;

    [Header("Bars")]
    [SerializeField] private GameObject strikersBar;
    [SerializeField] private GameObject toysBar;
    [SerializeField] private GameObject itemsBar;
    //Add toys and items bars

    //Booleans
    private bool hubToggle = false;
    private void Start()
    {
        btnHub.onClick.AddListener(() => { ToggleHUB(); });
        btnStrikersTab.onClick.AddListener(() => { ToggleStrikersTab(); });
        btnToysTab.onClick.AddListener(() => { ToggleToysTab(); });
        btnItemsTab.onClick.AddListener(() => { ToggleItemsTab(); });

        parentUI.SetActive(hubToggle);
        strikersBar.SetActive(true);
    }
    private void ToggleHUB()
    {
        hubToggle= !hubToggle;
        parentUI.gameObject.SetActive(hubToggle);
    }
    private void ToggleStrikersTab()
    {
        strikersBar.SetActive(true);
        toysBar.SetActive(false);
        itemsBar.SetActive(false);
    }
    private void ToggleToysTab()
    {
        toysBar.SetActive(true);
        strikersBar.SetActive(false);
        itemsBar.SetActive(false);
    }
    private void ToggleItemsTab()
    {
        itemsBar.SetActive(true);
        strikersBar.SetActive(false);
        toysBar.SetActive(false);
    }
}
