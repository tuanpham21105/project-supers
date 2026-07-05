using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeWindowUiController : WindowUiController
{
    [Serializable]
    class CharacterCustomizeTypeWindow
    {
        public CharacterCustomizeType type;
        public GameObject itemsListWindow;
        public GameObject propertiesWindow;

        public void SetActive(bool state)
        {
            if (itemsListWindow != null)
            {
                itemsListWindow.SetActive(state);
            }

            if (propertiesWindow != null)
                propertiesWindow.SetActive(state);
        }
    
        public void SetSelectedItem(string itemCode)
        {
            itemsListWindow.GetComponent<CharacterCustomizeItemsListUiController>().SetSelectedItemCode(itemCode != null ? itemCode : "");
        }
    }

    [SerializeField] private List<CharacterCustomizeTypeButtonUiController> characterCustomizeTypeButtons = new List<CharacterCustomizeTypeButtonUiController>();

    [SerializeField] private List<CharacterCustomizeTypeWindow> characterCustomizeTypeWindows = new List<CharacterCustomizeTypeWindow>();
    [SerializeField] private CharacterCustomizeTypeWindow openedCharacterCustomizeTypeWindows;

    [SerializeField] private CharacterCustomizeSkinCustomiziesUiController skinCustomiziesUiController;
    [SerializeField] private CharacterCustomizeEyesCustomiziesUiController eyesCustomiziesUiController;
    [SerializeField] private CharacterCustomizeHairCustomiziesUiController hairCustomiziesUiController;

    [SerializeField] CharacterCustomiziesSet tempCharacterCustomizies = new CharacterCustomiziesSet();

    void OnDestroy()
    {
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        foreach (CharacterCustomizeTypeButtonUiController a in characterCustomizeTypeButtons)
        {
            a.onClick += handleTypeButtonClick;
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            a.itemsListWindow.GetComponent<CharacterCustomizeItemsListUiController>().onItemSelected += handleItemSelected;
        }

        skinCustomiziesUiController.onPickColor += handlePickRacesColor;
        eyesCustomiziesUiController.onPickColor += handlePickEyesColor;
        hairCustomiziesUiController.onPickColor += handlePickHairColors;

        tempCharacterCustomizies = PlayerData.instance.characterCustomizies.Clone();

        OpenContentByType(CharacterCustomizeType.Races);
    }

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        foreach (CharacterCustomizeTypeButtonUiController a in characterCustomizeTypeButtons)
        {
            a.onClick -= handleTypeButtonClick;
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            a.itemsListWindow.GetComponent<CharacterCustomizeItemsListUiController>().onItemSelected -= handleItemSelected;
        }

        skinCustomiziesUiController.onPickColor -= handlePickRacesColor;
        eyesCustomiziesUiController.onPickColor -= handlePickEyesColor;
        hairCustomiziesUiController.onPickColor -= handlePickHairColors;

        Save();
    }

    void Save()
    {
        PlayerInventoryService.instance.SavePlayerCharacter
        (
            tempCharacterCustomizies.convertToRequest(),
            (response) =>
            {
                PlayerData.instance.characterCustomizies = tempCharacterCustomizies.Clone();

                Debug.Log($"Success to save character customize.");
            },
            (code, error) =>
            {
                Debug.LogError($"Failed to save character customize: {error}");
                MainMenuCharacterModelController.instance.SetCharacterCustomiziesFromPlayerData();
            }
        );
    }

    void handleTypeButtonClick(CharacterCustomizeType type)
    {
        OpenContentByType(type);
    }

    void OpenContentByType(CharacterCustomizeType type)
    {
        if (openedCharacterCustomizeTypeWindows != null)
        {
            openedCharacterCustomizeTypeWindows.SetActive(false);
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            if (type == a.type)
            {
                openedCharacterCustomizeTypeWindows = a;
                a.SetSelectedItem(GetItemCodeByCustomizeType(type));
                SetCustomize(a, type);
                a.SetActive(true);
            }
        }
    }

    void SetCustomize(CharacterCustomizeTypeWindow typeWindow, CharacterCustomizeType type)
    {
        switch (type)
        {
            case CharacterCustomizeType.Races:
                typeWindow.propertiesWindow.GetComponent<CharacterCustomizeSkinCustomiziesUiController>().SetColor(tempCharacterCustomizies.races.skinColor);
                break;
            case CharacterCustomizeType.Eyes:
                CharacterCustomizeEyes eyes = tempCharacterCustomizies.eyes;
                typeWindow.propertiesWindow.GetComponent<CharacterCustomizeEyesCustomiziesUiController>().SetColors(eyes.irisColor, eyes.scleraColor, eyes.eyebrowEyelidColor);
                break;
            case CharacterCustomizeType.Front_Hair:
                CharacterCustomizeHair frontHair = tempCharacterCustomizies.frontHair;
                typeWindow.propertiesWindow.GetComponent<CharacterCustomizeHairCustomiziesUiController>().SetColors(frontHair.primaryColor, frontHair.secondaryColor, frontHair.tertiaryColor);
                break;
            case CharacterCustomizeType.Top_Hair:
                CharacterCustomizeHair topHair = tempCharacterCustomizies.frontHair;
                typeWindow.propertiesWindow.GetComponent<CharacterCustomizeHairCustomiziesUiController>().SetColors(topHair.primaryColor, topHair.secondaryColor, topHair.tertiaryColor);
                break;
            case CharacterCustomizeType.Side_Hair:
                CharacterCustomizeHair sideHair = tempCharacterCustomizies.frontHair;
                typeWindow.propertiesWindow.GetComponent<CharacterCustomizeHairCustomiziesUiController>().SetColors(sideHair.primaryColor, sideHair.secondaryColor, sideHair.tertiaryColor);
                break;

        }
    }

    string GetItemCodeByCustomizeType(CharacterCustomizeType type)
    {
        CharacterCustomize customize = tempCharacterCustomizies.GetCharacterCustomizeByCustomizeType(type);

        if (customize == null || customize.itemSO == null)
            return null;
        
        return customize.itemSO.code;
    }

    void handleItemSelected(CharacterCustomizeItemSO itemSO)
    {
        tempCharacterCustomizies.GetCharacterCustomizeByCustomizeType(itemSO.type).itemSO = itemSO;

        MainMenuCharacterModelController.instance.SetCharacterCustomize(itemSO);
    }

    void handlePickRacesColor(Color skinColor)
    {
        tempCharacterCustomizies.races.skinColor = skinColor;

        MainMenuCharacterModelController.instance.SetCharacterRacesColor(skinColor);
    }

    void handlePickEyesColor(Color iris, Color sclera, Color eyelid)
    {
        tempCharacterCustomizies.eyes.irisColor = iris;
        tempCharacterCustomizies.eyes.scleraColor = sclera;
        tempCharacterCustomizies.eyes.eyebrowEyelidColor = eyelid;

        MainMenuCharacterModelController.instance.SetCharacterEyesColors(iris, sclera, eyelid);
    }

    void handlePickHairColors(Color primary, Color secondary, Color tertiary)
    {
        tempCharacterCustomizies.frontHair.primaryColor = primary;
        tempCharacterCustomizies.frontHair.secondaryColor = secondary;
        tempCharacterCustomizies.frontHair.tertiaryColor = tertiary;

        tempCharacterCustomizies.topHair.primaryColor = primary;
        tempCharacterCustomizies.topHair.secondaryColor = secondary;
        tempCharacterCustomizies.topHair.tertiaryColor = tertiary;

        tempCharacterCustomizies.sideHair.primaryColor = primary;
        tempCharacterCustomizies.sideHair.secondaryColor = secondary;
        tempCharacterCustomizies.sideHair.tertiaryColor = tertiary;

        MainMenuCharacterModelController.instance.SetCharacterHairColors(primary, secondary, tertiary);
    }
}
