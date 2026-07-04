using System;
using Newtonsoft.Json;
using UnityEngine;

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

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static CharacterCustomiziesSet FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomiziesSet>(json);
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
    public Color skinColor;

    public new CharacterCustomizeRaces Clone()
    {
        return new CharacterCustomizeRaces
        {
            itemSO = itemSO,
            skinColor = skinColor,
        };
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
    public Color pupilColor;
    public Color irisColor;
    public Color scleraColor;
    public Color eyebrowEyelidColor;

    public new CharacterCustomizeEyes Clone()
    {
        return new CharacterCustomizeEyes
        {
            itemSO = itemSO,
            pupilColor = pupilColor,
            irisColor = irisColor,
            scleraColor = scleraColor,
            eyebrowEyelidColor = eyebrowEyelidColor,
        };
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
    public Color primaryColor;
    public Color secondaryColor;
    public Color tertiaryColor;

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

    public new string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public new static CharacterCustomizeHair FromJson(string json)
    {
        return JsonConvert.DeserializeObject<CharacterCustomizeHair>(json);
    }
}