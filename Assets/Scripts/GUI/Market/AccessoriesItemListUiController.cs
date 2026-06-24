using System;
using System.Collections.Generic;
using UnityEngine;

public class AccessoriesItemListUiController : WindowUiController
{
    [SerializeField] private GameObject itemPrefab;
    
    [SerializeField] private Transform itemListObject;

    private Dictionary<String, AccessoriesItemUiController> itemUis = new Dictionary<String, AccessoriesItemUiController>();

    [SerializeField] private String selectedItemCode = "";

    private StoreItemsType type;
    public StoreItemsType GetListType() => type;

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();
    }

    void OnDestroy()
    {
        foreach (var item in itemUis)
        {
            item.Value.onSelected -= handleItemSelect;
        }
    }

    public void SetupItemsIntoList(StoreItemsType type, StoreItemsListResponse remoteList, AccessoriesListSO localList)
    {
        this.type = type;

        if (remoteList.itemsList == null)
            return;

        foreach (StoreItemResponse remoteItem in remoteList.itemsList)
        {
            AccessoryItemSO localItem = localList.findByCode(remoteItem.itemCode);
            if (localItem == null) continue;

            GameObject itemObject = Instantiate(itemPrefab, itemListObject);
            AccessoriesItemUiController itemController = itemObject.GetComponent<AccessoriesItemUiController>();
            itemController.SetItem(localItem.code, localItem.image, remoteItem.price, remoteItem.owned);
            itemController.onSelected += handleItemSelect;

            itemUis.Add(localItem.code, itemController);
        }
    }

    void handleItemSelect(String itemCode)
    {
        SetSelectedItem(itemCode);
        StoreUiController.instance.SelectItem(type, itemCode);
    }

    public void SetSelectedItem(String itemCode)
    {
        selectedItemCode = itemCode;

        foreach (var kvp in itemUis)
        {
            kvp.Value.SetSelected(kvp.Key == itemCode);
        }
    }

    public int GetItemPrice(string itemCode)
    {
        if (itemUis.TryGetValue(itemCode, out AccessoriesItemUiController item))
            return item.GetPrice();
        return 0;
    }
}