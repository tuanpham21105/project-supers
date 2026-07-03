using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeItemsListUiController : MonoBehaviour
{
    // [Header("Depedencies")]
    // [SerializeField] public CharacterCustomizeWindowUiController characterCustomizeWindowUiController;

    [Header("Objects")]
    [SerializeField] private Transform content;

    [Header("Resources")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private CharacterCustomizeItemsListSO itemsListSO;

    [Header("Runtime")]
    [SerializeField] private CharacterCustomizeItemButtonUiController selectedItem;
    // [SerializeField] private Dictionary<string, GameObject> itemButtons;

    // 
    public event Action<CharacterCustomizeItemSo> onItemSelected;
    
    void OnEnable()
    {
        if (selectedItem == null)
        {
            foreach (CharacterCustomizeItemSo item in itemsListSO.items)
            {
                GameObject newItem = Instantiate(itemPrefab, transform);

                newItem.GetComponent<CharacterCustomizeItemButtonUiController>().Initialize(item);
                newItem.GetComponent<CharacterCustomizeItemButtonUiController>().onSelected += handleItemSelected;

                // itemButtons.Add(item.code, newItem);
            }
        }
    }

    void handleItemSelected(CharacterCustomizeItemButtonUiController item)
    {
        SetSelectedItem(item);

        onItemSelected?.Invoke(item.itemSO);
    }

    public void SetSelectedItem(CharacterCustomizeItemButtonUiController item)
    {
        if (selectedItem != null)
        {
            selectedItem.SetSelected(false);
        }

        selectedItem = item;

        selectedItem.SetSelected(true);
    }
}
