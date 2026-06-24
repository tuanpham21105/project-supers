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

    public void SaveAccessories()
    {
        if (PlayerData.instance.points < totalCost)
        {
            Debug.LogError("Point insufficient");
            return;
        }

        PlayerAccessoriesSetRequest request = BuildAccessoriesSetRequest();

        PlayerInventoryService.instance.SavePlayerAccessoriesSet(
            request,
            (PlayerAccessoriesSetResponse response) =>
            {
                PlayerData.instance.points -= totalCost;
                totalCost = 0;
                totalCostTextField.text = "0";

                SetItemsOwnedFromResponse(response);

                PlayerData.instance.characterAccessories = storeAccessories;
            },
            (long errorCode, string errorMessage) =>
            {
                Debug.LogError($"Failed to save accessories: {errorMessage}");
            }
        );
    }

    private PlayerAccessoriesSetRequest BuildAccessoriesSetRequest()
    {
        return new PlayerAccessoriesSetRequest
        {
            hatItem = BuildAccessoryItemRequest(StoreItemsType.Hat),
            maskItem = BuildAccessoryItemRequest(StoreItemsType.Mask),
            neckItem = BuildAccessoryItemRequest(StoreItemsType.Neck),
            chestItem = BuildAccessoryItemRequest(StoreItemsType.Chest),
            backItem = BuildAccessoryItemRequest(StoreItemsType.Back),
            shouldersItem = BuildAccessoryItemRequest(StoreItemsType.Shoulders),
            glovesItem = BuildAccessoryItemRequest(StoreItemsType.Gloves),
            hipItem = BuildAccessoryItemRequest(StoreItemsType.Hip),
            legItem = BuildAccessoryItemRequest(StoreItemsType.Leg),
            bootsItem = BuildAccessoryItemRequest(StoreItemsType.Boots),
        };
    }

    private PlayerAccessoryItemRequest BuildAccessoryItemRequest(StoreItemsType type)
    {
        CharacterAccessory accessory = storeAccessories.TypeToAccessory(type);
        if (string.IsNullOrEmpty(accessory.itemCode)) return null;

        return new PlayerAccessoryItemRequest
        {
            itemCode = accessory.itemCode,
            properties = accessory.properties.ToJson()
        };
    }

    private void SetItemsOwnedFromResponse(PlayerAccessoriesSetResponse response)
    {
        SetItemOwned(StoreItemsType.Hat, response.hatItem?.itemCode);
        SetItemOwned(StoreItemsType.Mask, response.maskItem?.itemCode);
        SetItemOwned(StoreItemsType.Neck, response.neckItem?.itemCode);
        SetItemOwned(StoreItemsType.Chest, response.chestItem?.itemCode);
        SetItemOwned(StoreItemsType.Back, response.backItem?.itemCode);
        SetItemOwned(StoreItemsType.Shoulders, response.shouldersItem?.itemCode);
        SetItemOwned(StoreItemsType.Gloves, response.glovesItem?.itemCode);
        SetItemOwned(StoreItemsType.Hip, response.hipItem?.itemCode);
        SetItemOwned(StoreItemsType.Leg, response.legItem?.itemCode);
        SetItemOwned(StoreItemsType.Boots, response.bootsItem?.itemCode);
    }

    private void SetItemOwned(StoreItemsType type, string itemCode)
    {
        if (string.IsNullOrEmpty(itemCode)) return;
        if (itemListsDictionary.TryGetValue(type, out AccessoriesItemListUiController list))
        {
            list.SetItemOwned(itemCode);
        }
    }
}
