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
    [SerializeField] private string selectedItemCode;
    [SerializeField] private CharacterCustomizeItemButtonUiController selectedItem;
    [SerializeField] private Dictionary<string, CharacterCustomizeItemButtonUiController> itemButtons = new Dictionary<string, CharacterCustomizeItemButtonUiController>();

    // 
    public event Action<CharacterCustomizeItemSO> onItemSelected;
    
    void OnEnable()
    {
        if (selectedItem == null)
        {
            foreach (CharacterCustomizeItemSO item in itemsListSO.items)
            {
                Debug.Log(item.code);

                GameObject newItem = Instantiate(itemPrefab, content);

                CharacterCustomizeItemButtonUiController component = newItem.GetComponent<CharacterCustomizeItemButtonUiController>();

                component.Initialize(item);
                component.onSelected += handleItemSelected;

                itemButtons.Add(item.code, component);

                if (item.code.Equals(selectedItemCode))
                {
                    SetSelectedItem(component);
                }
            }
        }
        else
        {
            SetSelectedItem(itemButtons[selectedItemCode]);
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

    public void SetSelectedItemCode(string code)
    {
        selectedItemCode = code;
    }
}
