using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCharacterModelController : MonoBehaviour
{
    public static MainMenuCharacterModelController instance;

    [SerializeField] private CharacterAccessoriesController characterAccessoriesController;

    void Awake()
    {
        instance = this;
    }

    public void PutOnAccessory(AccessoryItemSO itemSo)
    {
        if (itemSo == null)
            return;
        
        GameObject item = Instantiate(itemSo.itemPrefab);

        characterAccessoriesController.PutOnAccessories(item.GetComponent<CharacterAccessoryItemData>());
    }

    public void SetPlayerCharacterAccessoriesFromPlayerData()
    {
        PutOnAccessory(GetAccessorySO(StoreItemsType.Hat));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Mask));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Neck));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Chest));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Back));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Shoulders));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Gloves));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Hip));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Leg));
        PutOnAccessory(GetAccessorySO(StoreItemsType.Boots));
    }

    AccessoryItemSO GetAccessorySO(StoreItemsType type)
    {
        CharacterAccessory accessory = PlayerData.instance.characterAccessories.TypeToAccessory(type);
        return StoreData.instance.GetLocalListByType(type).findByCode(accessory.itemCode);
    }
}
