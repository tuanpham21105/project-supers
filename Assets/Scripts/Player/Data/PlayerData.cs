using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterAccessoriesSet
{
    public CharacterAccessory hatItem = new CharacterAccessory();
    public CharacterAccessory maskItem = new CharacterAccessory();
    public CharacterAccessory neckItem = new CharacterAccessory();
    public CharacterAccessory chestItem = new CharacterAccessory();
    public CharacterAccessory backItem = new CharacterAccessory();
    public CharacterAccessory shouldersItem = new CharacterAccessory();
    public CharacterAccessory glovesItem = new CharacterAccessory();
    public CharacterAccessory hipItem = new CharacterAccessory();
    public CharacterAccessory legItem = new CharacterAccessory();
    public CharacterAccessory bootsItem = new CharacterAccessory();

    public CharacterAccessory TypeToAccessory(StoreItemsType type)
    {
        return type switch
        {
            StoreItemsType.Hat => hatItem,
            StoreItemsType.Mask => maskItem,
            StoreItemsType.Neck => neckItem,
            StoreItemsType.Chest => chestItem,
            StoreItemsType.Back => backItem,
            StoreItemsType.Shoulders => shouldersItem,
            StoreItemsType.Gloves => glovesItem,
            StoreItemsType.Hip => hipItem,
            StoreItemsType.Leg => legItem,
            StoreItemsType.Boots => bootsItem,
            _ => null
        };
    }

    public CharacterAccessoriesSet Clone()
    {
        return new CharacterAccessoriesSet
        {
            hatItem = hatItem?.Clone(),
            maskItem = maskItem?.Clone(),
            neckItem = neckItem?.Clone(),
            chestItem = chestItem?.Clone(),
            backItem = backItem?.Clone(),
            shouldersItem = shouldersItem?.Clone(),
            glovesItem = glovesItem?.Clone(),
            hipItem = hipItem?.Clone(),
            legItem = legItem?.Clone(),
            bootsItem = bootsItem?.Clone(),
        };
    }
}

[Serializable]
public class CharacterAccessory
{
    public string itemCode = "";
    public AccessoryProperties properties = new AccessoryProperties();

    public CharacterAccessory Clone()
    {
        return new CharacterAccessory
        {
            itemCode = itemCode,
            properties = properties?.Clone(),
        };
    }
}

[Serializable]
public class AccessoryProperties
{
    public Color primaryColor = new Color();
    public Color secondaryColor = new Color();
    public Color tertiaryColor = new Color();

    public AccessoryProperties Clone()
    {
        return new AccessoryProperties
        {
            primaryColor = primaryColor,
            secondaryColor = secondaryColor,
            tertiaryColor = tertiaryColor,
        };
    }

    public string ToJson()
    {
        return "{\"primaryColor\":\"#" + ColorUtility.ToHtmlStringRGBA(primaryColor)
             + "\",\"secondaryColor\":\"#" + ColorUtility.ToHtmlStringRGBA(secondaryColor)
             + "\",\"tertiaryColor\":\"#" + ColorUtility.ToHtmlStringRGBA(tertiaryColor) + "\"}";
    }

    public static AccessoryProperties FromJson(string json)
    {
        AccessoryProperties properties = new AccessoryProperties();
        if (string.IsNullOrEmpty(json)) return properties;

        properties.primaryColor = ParseColor(json, "primaryColor");
        properties.secondaryColor = ParseColor(json, "secondaryColor");
        properties.tertiaryColor = ParseColor(json, "tertiaryColor");

        return properties;
    }

    private static Color ParseColor(string json, string key)
    {
        string hexKey = "\"" + key + "\":\"#";
        int startIndex = json.IndexOf(hexKey);
        if (startIndex < 0) return new Color();

        startIndex += hexKey.Length;
        int endIndex = json.IndexOf("\"", startIndex);
        if (endIndex < 0) return new Color();

        string hex = json.Substring(startIndex, endIndex - startIndex);
        ColorUtility.TryParseHtmlString("#" + hex, out Color parsedColor);
        return parsedColor;
    }
}

public class PlayerData : MonoBehaviour
{
    public static PlayerData instance;

    public String email;
    public String username;
    public String createdDate;
    public bool isGuest = false;

    private long points = 0;
    public long Points
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
            onPointsChange?.Invoke();
        }
    }
    public CharacterAccessoriesSet characterAccessories = new CharacterAccessoriesSet();

    public event Action onPointsChange;
    
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void updateUsername(string text)
    {
        username = text;
    }
}
