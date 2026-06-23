using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreUiController : WindowUiController
{
    public static StoreUiController instance;

    [SerializeField] private AccessoriesColorPalleteUiController accessoriesColorPalleteUiController;

    [SerializeField] private TextMeshProUGUI totalCostTextField;
    [SerializeField] private GameObject saveButtonObject;

    [SerializeField] private int totalCost;

    [SerializeField] private GameObject itemListPrefab;
    [SerializeField] private Transform itemsListSlot;
    [SerializeField] private AccessoriesItemListUiController currentItemList;
    [SerializeField] private Dictionary<StoreItemsType, AccessoriesItemListUiController> itemListsDictionary = new Dictionary<StoreItemsType, AccessoriesItemListUiController>();

    [SerializeField] private CharacterAccessoriesSet storeAccessories; 

    void Awake()
    {
        instance = this;
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        SetTotalCost(0);

        storeAccessories = PlayerData.instance.characterAccessories.Clone();

        SelectType(StoreItemsType.Hat);

        accessoriesColorPalleteUiController.SelectColorLevel(0);
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        MainMenuCharacterModelController.instance.SetPlayerCharacterAccessoriesFromPlayerData();
    }

    public void SelectType(StoreItemsType type)
    {
        if (itemListsDictionary.ContainsKey(type))
        {
            currentItemList.CloseWindow();

            currentItemList = itemListsDictionary[type];

            currentItemList.OpenWindow();

            currentItemList.SetSelectedItem(storeAccessories.TypeToAccessory(type).itemCode);
        }
        else
        {
            AccessoriesListSO localList = StoreData.instance.GetLocalListByType(type);
            if (localList == null) return;

            StoreService.instance.GetStoreItemsByType(type,
                (StoreItemsListResponse response) =>
                {
                    GameObject newListObject = Instantiate(itemListPrefab, itemsListSlot);
                    AccessoriesItemListUiController newList = newListObject.GetComponent<AccessoriesItemListUiController>();
                    newList.SetupItemsIntoList(type, response, localList);

                    itemListsDictionary[type] = newList;

                    currentItemList?.CloseWindow();
                    currentItemList = newList;
                    currentItemList.OpenWindow();

                    currentItemList.SetSelectedItem(storeAccessories.TypeToAccessory(type).itemCode);
                },
                (long errorCode, string errorMessage) =>
                {
                    Debug.LogError($"Failed to load items for type {type}: {errorMessage}");
                }
            );
        }
    
    }

    public void SelectItem(StoreItemsType type, String itemCode)
    {
        AccessoriesListSO localList = StoreData.instance.GetLocalListByType(type);
        AccessoryItemSO accessoryItemSO = localList.findByCode(itemCode);

        MainMenuCharacterModelController.instance.PutOnAccessory(accessoryItemSO);

        storeAccessories.TypeToAccessory(type).itemCode = itemCode;
    }

    public void UpdateTotalCost(int amount)
    {
        totalCost += amount;
        
        totalCostTextField.text = totalCost.ToString();
    }

    public void SetTotalCost(int totalCost)
    {
        this.totalCost = totalCost;
        
        totalCostTextField.text = totalCost.ToString();
    }
}
