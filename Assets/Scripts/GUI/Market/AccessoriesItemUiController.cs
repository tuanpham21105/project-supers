using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesItemUiController : MonoBehaviour
{
    [SerializeField] private string code;
    [SerializeField] private int price;
    [SerializeField] private bool isOwned = false;
    [SerializeField] private Image bgrImage;
    [SerializeField] private Image image;
    [SerializeField] private GameObject valueTitleObject;
    [SerializeField] private TextMeshProUGUI valueTextField;
    [SerializeField] private GameObject ownedTitleObject;

    [SerializeField] private Sprite normalBgrTexture;
    [SerializeField] private Sprite selectedBgrTexture;

    private AccessoryProperties accessoryProperties = null;

    public event Action<string, AccessoryProperties> onSelected;

    public void SetItem(string code, Sprite sprite, int value, bool isOwned, string properties)
    {
        this.code = code;
        this.image.sprite = sprite;
        price = value;
        valueTextField.text = BigNumberStringify.decorate(price);
        this.isOwned = isOwned;
        accessoryProperties = AccessoryProperties.FromJson(properties);
        if (isOwned) 
            SetOwned();
    }

    public void OnPress()
    {
        onSelected?.Invoke(code, accessoryProperties);
    }

    public void SetSelected(bool state)
    {
        bgrImage.sprite = state ? selectedBgrTexture : normalBgrTexture;
    }

    public void SetOwned()
    {
        valueTitleObject.SetActive(false);
        ownedTitleObject.SetActive(true);
        isOwned = true;
    }

    public int GetPrice() => isOwned ? 0 : price;

    public AccessoryProperties GetAccessoryProperties() => accessoryProperties;
}
