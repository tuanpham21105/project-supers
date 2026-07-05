using System;
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

    public void PutOnAccessory(AccessoryItemSO itemSo, AccessoryProperties properties)
    {
        characterAccessoriesController.PutOn(itemSo, properties);
    }

    public void ChangeAccessoryColors(StoreItemsType type, AccessoryProperties properties)
    {
        characterAccessoriesController.SetAccessoryColorsByPart(properties, Enum.Parse<AccessoriesPart>(type.ToString()));
    }

    public void SetPlayerCharacterAccessoriesFromPlayerData()
    {
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Hat);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Mask);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Neck);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Chest);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Back);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Shoulders);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Gloves);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Hip);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Leg);
        PutOnAccessoryFromPlayerDataByType(StoreItemsType.Boots);
    }

    void PutOnAccessoryFromPlayerDataByType(StoreItemsType type)
    {
        CharacterAccessory accessory = PlayerData.instance.characterAccessories.TypeToAccessory(type);
        
        PutOnAccessory(StoreData.instance.GetLocalListByType(type).findByCode(accessory.itemCode), accessory.properties);
    }

    //
    public void SetCharacterCustomize(CharacterCustomizeItemSO itemSO)
    {
        characterAccessoriesController.SetCharacterCustomize(itemSO);
    }

    public void SetCharacterRacesColor(Color skinColor)
    {
        characterAccessoriesController.SetRacesColor(skinColor);
    }

    public void SetCharacterEyesColors(Color irisColor, Color scleraColor, Color eyelidColor)
    {
        characterAccessoriesController.SetEyesColors(irisColor, scleraColor, eyelidColor);
    }

    public void SetCharacterHairColors(Color primary, Color secondary, Color tertiary)
    {
        characterAccessoriesController.SetHairColors(primary, secondary, tertiary);
    }
}
