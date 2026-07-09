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

    public static CharacterAccessoriesSet MapFromResponse(PlayerAccessoriesSetResponse response)
    {
        if (response == null) return new CharacterAccessoriesSet();

        return new CharacterAccessoriesSet
        {
            hatItem = CharacterAccessory.MapAccessoryFromResponse(response.hatItem),
            maskItem = CharacterAccessory.MapAccessoryFromResponse(response.maskItem),
            neckItem = CharacterAccessory.MapAccessoryFromResponse(response.neckItem),
            chestItem = CharacterAccessory.MapAccessoryFromResponse(response.chestItem),
            backItem = CharacterAccessory.MapAccessoryFromResponse(response.backItem),
            shouldersItem = CharacterAccessory.MapAccessoryFromResponse(response.shouldersItem),
            glovesItem = CharacterAccessory.MapAccessoryFromResponse(response.glovesItem),
            hipItem = CharacterAccessory.MapAccessoryFromResponse(response.hipItem),
            legItem = CharacterAccessory.MapAccessoryFromResponse(response.legItem),
            bootsItem = CharacterAccessory.MapAccessoryFromResponse(response.bootsItem),
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

    public static CharacterAccessory MapAccessoryFromResponse(PlayerAccessoryItemResponse response)
    {
        if (response == null) return new CharacterAccessory();

        return new CharacterAccessory
        {
            itemCode = response.itemCode,
            properties = AccessoryProperties.FromJson(response.properties),
        };
    }
}

[Serializable]
public class AccessoryProperties
{
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.white;
    public Color tertiaryColor = Color.white;

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
        if (string.IsNullOrEmpty(json)) return null;

        AccessoryProperties properties = new AccessoryProperties();

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

    public string email = "";
    public string username = "";
    public string createdDate;
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
    public CharacterCustomiziesSet characterCustomizies = new CharacterCustomiziesSet();

    public int levels;
    public long exp;
    public long levelsUpExp;

    private int masterVolume = 100;

    public int MasterVolume
    {
        get
        {
            return masterVolume;
        }
        set
        {
            masterVolume = value;
            onMasterVolumeChange?.Invoke(masterVolume);
        }
    }

    public event Action onPointsChange;
    public event Action<int> onMasterVolumeChange;
    
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

    public void Logout()
    {
        Destroy(gameObject);
    }
}
