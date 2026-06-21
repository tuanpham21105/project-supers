using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreUiController : WindowUiController
{
    public static StoreUiController instance;

    [SerializeField] private AccessoriesListSO localHatList;
    [SerializeField] private AccessoriesListSO localMaskList;
    [SerializeField] private AccessoriesListSO localNeckList;
    [SerializeField] private AccessoriesListSO localChestList;
    [SerializeField] private AccessoriesListSO localBackList;
    [SerializeField] private AccessoriesListSO localShouldersList;
    [SerializeField] private AccessoriesListSO localGlovesList;
    [SerializeField] private AccessoriesListSO localHipList;
    [SerializeField] private AccessoriesListSO localLegList;
    [SerializeField] private AccessoriesListSO localBootsList;

    [SerializeField] private AccessoriesColorPalleteUiController accessoriesColorPalleteUiController;

    [SerializeField] private TextMeshProUGUI totalCostTextField;
    [SerializeField] private GameObject saveButtonObject;

    [SerializeField] private int totalCost;

    [SerializeField] private GameObject itemListPrefab;
    [SerializeField] private Transform itemsListSlot;
    [SerializeField] private AccessoriesItemListUiController currentItemList;
    [SerializeField] private Dictionary<StoreItemsType, AccessoriesItemListUiController> itemListsDictionary = new Dictionary<StoreItemsType, AccessoriesItemListUiController>();

    

    void Awake()
    {
        instance = this;
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        SelectType(StoreItemsType.Hat);

        accessoriesColorPalleteUiController.SelectColorLevel(0);
    }

    public void SelectType(StoreItemsType type)
    {
        if (itemListsDictionary.ContainsKey(type))
        {
            currentItemList.CloseWindow();

            currentItemList = itemListsDictionary[type];

            currentItemList.OpenWindow();
        }
        else
        {
            AccessoriesListSO localList = GetLocalListByType(type);
            if (localList == null) return;

            StoreService.instance.GetStoreItemsByType(type,
                (StoreItemsListResponse response) =>
                {
                    GameObject newListObject = Instantiate(itemListPrefab, itemsListSlot);
                    AccessoriesItemListUiController newList = newListObject.GetComponent<AccessoriesItemListUiController>();
                    newList.AddItemsIntoList(response, localList);

                    itemListsDictionary[type] = newList;

                    currentItemList?.CloseWindow();
                    currentItemList = newList;
                    currentItemList.OpenWindow();
                },
                (long errorCode, string errorMessage) =>
                {
                    Debug.LogError($"Failed to load items for type {type}: {errorMessage}");
                }
            );
        }
    }

    private AccessoriesListSO GetLocalListByType(StoreItemsType type)
    {
        return type switch
        {
            StoreItemsType.Hat => localHatList,
            StoreItemsType.Mask => localMaskList,
            StoreItemsType.Neck => localNeckList,
            StoreItemsType.Chest => localChestList,
            StoreItemsType.Back => localBackList,
            StoreItemsType.Shoulders => localShouldersList,
            StoreItemsType.Gloves => localGlovesList,
            StoreItemsType.Hip => localHipList,
            StoreItemsType.Leg => localLegList,
            StoreItemsType.Boots => localBootsList,
            _ => null
        };
    }

    public void SelectItem()
    {
        
    }

    public void UpdateTotalCost()
    {
        
    }
}
