using UnityEngine;

public class AccessoriesItemListUiController : WindowUiController
{
    [SerializeField] private GameObject itemPrefab;
    
    [SerializeField] private Transform itemListObject;

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        
    }

    public void AddItemsIntoList(StoreItemsListResponse remoteList, AccessoriesListSO localList)
    {
        foreach (StoreItemResponse remoteItem in remoteList.itemsList)
        {
            AccessoryItemSO localItem = localList.findByCode(remoteItem.itemCode);
            if (localItem == null) continue;

            GameObject itemObject = Instantiate(itemPrefab, itemListObject);
            AccessoriesItemUiController itemController = itemObject.GetComponent<AccessoriesItemUiController>();
            itemController.SetItem(localItem.code, null, remoteItem.price);
        }
    }
}