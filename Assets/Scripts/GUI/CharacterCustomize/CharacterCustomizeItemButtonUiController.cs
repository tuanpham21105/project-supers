using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCustomizeItemButtonUiController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image displayImage;

    [Header("Resources")]
    [SerializeField] private Sprite selectedBackgroundSprite;
    [SerializeField] private Sprite normalBackgroundSprite;

    [Header("Data")]
    [SerializeField] public CharacterCustomizeItemSO itemSO;
    [SerializeField] public bool isSelected = false;

    //
    public event Action<CharacterCustomizeItemButtonUiController> onSelected;

    public void Click()
    {
        if (isSelected)
            return;

        onSelected?.Invoke(this);
        SetSelected(true);
    }

    public void Initialize(CharacterCustomizeItemSO data)
    {
        itemSO = data;

        displayImage.sprite = itemSO.itemSprite;
    }

    public void SetSelected(bool state)
    {
        isSelected = state;
        backgroundImage.sprite = state ? selectedBackgroundSprite : normalBackgroundSprite;        
    }
}
