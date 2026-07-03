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
                itemsListWindow.SetActive(state);

            if (propertiesWindow != null)
                propertiesWindow.SetActive(state);
        }
    }

    [SerializeField] private List<CharacterCustomizeTypeButtonUiController> characterCustomizeTypeButtons;

    [SerializeField] private List<CharacterCustomizeTypeWindow> characterCustomizeTypeWindows;
    [SerializeField] private CharacterCustomizeTypeWindow openedCharacterCustomizeTypeWindows;

    public override void Initialize()
    {
        base.Initialize();

        foreach (CharacterCustomizeTypeButtonUiController a in characterCustomizeTypeButtons)
        {
            a.onClick += handleTypeButtonClick;
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            a.itemsListWindow.GetComponent<CharacterCustomizeItemsListUiController>().onItemSelected += handleItemSelected;
        }
    }

    void OnDestroy()
    {

        foreach (CharacterCustomizeTypeButtonUiController a in characterCustomizeTypeButtons)
        {
            a.onClick -= handleTypeButtonClick;
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            a.itemsListWindow.GetComponent<CharacterCustomizeItemsListUiController>().onItemSelected -= handleItemSelected;
        }
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        OpenContentByType(CharacterCustomizeType.Races);
    }

    void handleTypeButtonClick(CharacterCustomizeType type)
    {
        OpenContentByType(type);
    }

    void OpenContentByType(CharacterCustomizeType type)
    {
        if (openedCharacterCustomizeTypeWindows != null)
        {
            if (openedCharacterCustomizeTypeWindows.type == type)
                return;

                openedCharacterCustomizeTypeWindows.SetActive(false);
        }

        foreach (CharacterCustomizeTypeWindow a in characterCustomizeTypeWindows)
        {
            if (type == a.type)
            {
                openedCharacterCustomizeTypeWindows = a;
                a.SetActive(true);
            }
        }
    }

    void handleItemSelected(CharacterCustomizeItemSo itemSO)
    {
        
    }
}
