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
                a.SetActive(true);
            }
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
}
