using System;
using Newtonsoft.Json;
using UnityEngine;

public class ColorHexConverter : JsonConverter<Color>
{
    public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
            return existingValue;

        var hex = reader.Value as string;
        if (string.IsNullOrEmpty(hex))
            return default;

        if (!hex.StartsWith("#"))
            hex = "#" + hex;

        ColorUtility.TryParseHtmlString(hex, out Color color);
        return color;
    }

    public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
    {
        writer.WriteValue(ColorUtility.ToHtmlStringRGBA(value));
    }
}

[Serializable]
public class CharacterCustomiziesSet
{
    public CharacterCustomizeRaces races;
    public CharacterCustomizeEyes eyes;
    public CharacterCustomize mouth;
    public CharacterCustomizeHair topHair;
    public CharacterCustomizeHair frontHair;
    public CharacterCustomizeHair sideHair;

    public CharacterCustomiziesSet Clone()
    {
        return new CharacterCustomiziesSet
        {
            races = races?.Clone(),
            eyes = eyes?.Clone(),
            mouth = mouth?.Clone(),
            topHair = topHair?.Clone(),
            frontHair = frontHair?.Clone(),
            sideHair = sideHair?.Clone(),
        };
    }

    public CharacterCustomize GetCharacterCustomizeByCustomizeType(CharacterCustomizeType type)
    {
        switch(type)
        {
            case CharacterCustomizeType.Races:
                return races;
            case CharacterCustomizeType.Eyes:
                return eyes;
            case CharacterCustomizeType.Mouth:
                return mouth;
            case CharacterCustomizeType.Front_Hair:
                return frontHair;
            case CharacterCustomizeType.Top_Hair:
                return topHair;
            case CharacterCustomizeType.Side_Hair:
                return sideHair;
            default:
                return null;
        }
    }

    public PlayerCharacterRequest convertToRequest()
    {
        return new PlayerCharacterRequest
        {
            racesCode = races?.itemSO?.code,
            racesProperties = races?.ToPropertiesJson(),
            eyesCode = eyes?.itemSO?.code,
            eyesProperties = eyes?.ToPropertiesJson(),
            mouthCode = mouth?.itemSO?.code,
            frontHairCode = frontHair?.itemSO?.code,
            frontHairProperties = frontHair?.ToPropertiesJson(),
            topHairCode = topHair?.itemSO?.code,
            topHairProperties = topHair?.ToPropertiesJson(),
            sideHairCode = sideHair?.itemSO?.code,
            sideHairProperties = sideHair?.ToPropertiesJson(),
        };
    }

    public void convertFromResponse(PlayerCharacterResponse response)
    {
        if (response.racesCode != null)
        {
            races = new CharacterCustomizeRaces();
            races.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Races).findByCode(response.racesCode);
            if (response.racesProperties != null)
                races.FromPropertiesJson(response.racesProperties);
        }

        if (response.eyesCode != null)
        {
            eyes = new CharacterCustomizeEyes();
            eyes.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Eyes).findByCode(response.eyesCode);
            if (response.eyesProperties != null)
                eyes.FromPropertiesJson(response.eyesProperties);
        }

        if (response.mouthCode != null)
        {
            mouth = new CharacterCustomize();
            mouth.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Mouth).findByCode(response.mouthCode);
        }

        if (response.frontHairCode != null)
        {
            frontHair = new CharacterCustomizeHair();
            frontHair.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Front_Hair).findByCode(response.frontHairCode);
            if (response.frontHairProperties != null)
                frontHair.FromPropertiesJson(response.frontHairProperties);
        }

        if (response.topHairCode != null)
        {
            topHair = new CharacterCustomizeHair();
            topHair.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Top_Hair).findByCode(response.topHairCode);
            if (response.topHairProperties != null)
                topHair.FromPropertiesJson(response.topHairProperties);
        }

        if (response.sideHairCode != null)
        {
            sideHair = new CharacterCustomizeHair();
            sideHair.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Side_Hair).findByCode(response.sideHairCode);
            if (response.sideHairProperties != null)
                sideHair.FromPropertiesJson(response.sideHairProperties);
        }
    }
}

[Serializable]
public class CharacterCustomize
{
    public CharacterCustomizeItemSO itemSO;

    public CharacterCustomize Clone()
    {
        return new CharacterCustomize
        {
            itemSO = itemSO,
        };
    }

    public virtual string ToPropertiesJson() => "{}";
    public virtual void FromPropertiesJson(string json) { }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static CharacterCustomize FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomize>(json);
    }
}

[Serializable]
public class CharacterCustomizeRaces : CharacterCustomize
{
    [JsonConverter(typeof(ColorHexConverter))]
    public Color skinColor = Color.red;

    public new CharacterCustomizeRaces Clone()
    {
        return new CharacterCustomizeRaces
        {
            itemSO = itemSO,
            skinColor = skinColor,
        };
    }

    public new string ToPropertiesJson()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ColorHexConverter());
        return JsonConvert.SerializeObject(new { skinColor }, settings);
    }

    public new void FromPropertiesJson(string json)
    {
        JsonConvert.PopulateObject(json, this);
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public new static CharacterCustomizeRaces FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomizeRaces>(json);
    }
}

[Serializable]
public class CharacterCustomizeEyes : CharacterCustomize
{
    // public Color pupilColor;
    [JsonConverter(typeof(ColorHexConverter))]
    public Color irisColor = Color.black;
    [JsonConverter(typeof(ColorHexConverter))]
    public Color scleraColor = Color.white;
    [JsonConverter(typeof(ColorHexConverter))]
    public Color eyebrowEyelidColor = Color.black;

    public new CharacterCustomizeEyes Clone()
    {
        return new CharacterCustomizeEyes
        {
            itemSO = itemSO,
            // pupilColor = pupilColor,
            irisColor = irisColor,
            scleraColor = scleraColor,
            eyebrowEyelidColor = eyebrowEyelidColor,
        };
    }

    public new string ToPropertiesJson()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ColorHexConverter());
        return JsonConvert.SerializeObject(new { irisColor, scleraColor, eyebrowEyelidColor }, settings);
    }

    public new void FromPropertiesJson(string json)
    {
        JsonConvert.PopulateObject(json, this);
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public new static CharacterCustomizeEyes FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomizeEyes>(json);
    }
}

[Serializable]
public class CharacterCustomizeHair : CharacterCustomize
{
    [JsonConverter(typeof(ColorHexConverter))]
    public Color primaryColor = Color.black;
    [JsonConverter(typeof(ColorHexConverter))]
    public Color secondaryColor = Color.black;
    [JsonConverter(typeof(ColorHexConverter))]
    public Color tertiaryColor = Color.black;

    public new CharacterCustomizeHair Clone()
    {
        return new CharacterCustomizeHair
        {
            itemSO = itemSO,
            primaryColor = primaryColor,
            secondaryColor = secondaryColor,
            tertiaryColor = tertiaryColor,
        };
    }

    public new string ToPropertiesJson()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new ColorHexConverter());
        return JsonConvert.SerializeObject(new { primaryColor, secondaryColor, tertiaryColor }, settings);
    }

    public new void FromPropertiesJson(string json)
    {
        JsonConvert.PopulateObject(json, this);
    }

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public new static CharacterCustomizeHair FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomizeHair>(json);
    }
}