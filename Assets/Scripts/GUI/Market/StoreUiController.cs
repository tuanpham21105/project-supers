using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        storeAccessories = PlayerData.instance.characterAccessories.Clone();

        SelectType(StoreItemsType.Hat);

        accessoriesColorPalleteUiController.SelectColorLevel(0);

        RecalculateTotalCost();
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

                    CharacterAccessory accessory = storeAccessories.TypeToAccessory(type);
                    string itemCode = accessory.itemCode;

                    currentItemList.SetSelectedItem(itemCode);

                    RecalculateTotalCost();
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

        RecalculateTotalCost();
    }

    void RecalculateTotalCost()
    {
        totalCost = 0;

        foreach (StoreItemsType type in Enum.GetValues(typeof(StoreItemsType)))
        {
            if (type == StoreItemsType.Skills) continue;

            string itemCode = storeAccessories.TypeToAccessory(type).itemCode;
            if (string.IsNullOrEmpty(itemCode)) continue;

            if (itemListsDictionary.TryGetValue(type, out AccessoriesItemListUiController list))
            {
                totalCost += list.GetItemPrice(itemCode);
            }
        }

        totalCostTextField.text = totalCost.ToString();
        bool canAfford = totalCost <= PlayerData.instance.points;
        totalCostTextField.color = canAfford ? Color.white : Color.red;
        saveButtonObject.GetComponent<Button>().interactable = canAfford;
    }
}
