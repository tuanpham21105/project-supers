using System;
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
            racesProperties = races != null ? new RacesPropertiesValueObject
            {
                skinColor = ColorUtility.ToHtmlStringRGBA(races.skinColor)
            } : null,
            eyesCode = eyes?.itemSO?.code,
            eyesProperties = eyes != null ? new EyesPropertiesValueObject
            {
                irisColor = ColorUtility.ToHtmlStringRGBA(eyes.irisColor),
                scleraColor = ColorUtility.ToHtmlStringRGBA(eyes.scleraColor),
                eyebrowEyelidColor = ColorUtility.ToHtmlStringRGBA(eyes.eyebrowEyelidColor)
            } : null,
            mouthCode = mouth?.itemSO?.code,
            frontHairCode = frontHair?.itemSO?.code,
            frontHairProperties = frontHair != null ? new HairPropertiesValueObject
            {
                primaryColor = ColorUtility.ToHtmlStringRGBA(frontHair.primaryColor),
                secondaryColor = ColorUtility.ToHtmlStringRGBA(frontHair.secondaryColor),
                tertiaryColor = ColorUtility.ToHtmlStringRGBA(frontHair.tertiaryColor)
            } : null,
            topHairCode = topHair?.itemSO?.code,
            topHairProperties = topHair != null ? new HairPropertiesValueObject
            {
                primaryColor = ColorUtility.ToHtmlStringRGBA(topHair.primaryColor),
                secondaryColor = ColorUtility.ToHtmlStringRGBA(topHair.secondaryColor),
                tertiaryColor = ColorUtility.ToHtmlStringRGBA(topHair.tertiaryColor)
            } : null,
            sideHairCode = sideHair?.itemSO?.code,
            sideHairProperties = sideHair != null ? new HairPropertiesValueObject
            {
                primaryColor = ColorUtility.ToHtmlStringRGBA(sideHair.primaryColor),
                secondaryColor = ColorUtility.ToHtmlStringRGBA(sideHair.secondaryColor),
                tertiaryColor = ColorUtility.ToHtmlStringRGBA(sideHair.tertiaryColor)
            } : null,
        };
    }

    public void convertFromResponse(PlayerCharacterResponse response)
    {
        if (response.racesCode != null)
        {
            races = new CharacterCustomizeRaces();
            races.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Races).findByCode(response.racesCode);
            if (response.racesProperties != null && !string.IsNullOrEmpty(response.racesProperties.skinColor))
                ColorUtility.TryParseHtmlString("#" + response.racesProperties.skinColor, out races.skinColor);
        }

        if (response.eyesCode != null)
        {
            eyes = new CharacterCustomizeEyes();
            eyes.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Eyes).findByCode(response.eyesCode);
            if (response.eyesProperties != null)
            {
                if (!string.IsNullOrEmpty(response.eyesProperties.irisColor))
                    ColorUtility.TryParseHtmlString("#" + response.eyesProperties.irisColor, out eyes.irisColor);
                if (!string.IsNullOrEmpty(response.eyesProperties.scleraColor))
                    ColorUtility.TryParseHtmlString("#" + response.eyesProperties.scleraColor, out eyes.scleraColor);
                if (!string.IsNullOrEmpty(response.eyesProperties.eyebrowEyelidColor))
                    ColorUtility.TryParseHtmlString("#" + response.eyesProperties.eyebrowEyelidColor, out eyes.eyebrowEyelidColor);
            }
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
            {
                if (!string.IsNullOrEmpty(response.frontHairProperties.primaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.frontHairProperties.primaryColor, out frontHair.primaryColor);
                if (!string.IsNullOrEmpty(response.frontHairProperties.secondaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.frontHairProperties.secondaryColor, out frontHair.secondaryColor);
                if (!string.IsNullOrEmpty(response.frontHairProperties.tertiaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.frontHairProperties.tertiaryColor, out frontHair.tertiaryColor);
            }
        }

        if (response.topHairCode != null)
        {
            topHair = new CharacterCustomizeHair();
            topHair.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Top_Hair).findByCode(response.topHairCode);
            if (response.topHairProperties != null)
            {
                if (!string.IsNullOrEmpty(response.topHairProperties.primaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.topHairProperties.primaryColor, out topHair.primaryColor);
                if (!string.IsNullOrEmpty(response.topHairProperties.secondaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.topHairProperties.secondaryColor, out topHair.secondaryColor);
                if (!string.IsNullOrEmpty(response.topHairProperties.tertiaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.topHairProperties.tertiaryColor, out topHair.tertiaryColor);
            }
        }

        if (response.sideHairCode != null)
        {
            sideHair = new CharacterCustomizeHair();
            sideHair.itemSO = StoreData.instance.GetCustomizeListByType(CharacterCustomizeType.Side_Hair).findByCode(response.sideHairCode);
            if (response.sideHairProperties != null)
            {
                if (!string.IsNullOrEmpty(response.sideHairProperties.primaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.sideHairProperties.primaryColor, out sideHair.primaryColor);
                if (!string.IsNullOrEmpty(response.sideHairProperties.secondaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.sideHairProperties.secondaryColor, out sideHair.secondaryColor);
                if (!string.IsNullOrEmpty(response.sideHairProperties.tertiaryColor))
                    ColorUtility.TryParseHtmlString("#" + response.sideHairProperties.tertiaryColor, out sideHair.tertiaryColor);
            }
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
}

[Serializable]
public class CharacterCustomizeRaces : CharacterCustomize
{
    public Color skinColor = Color.red;

    public new CharacterCustomizeRaces Clone()
    {
        return new CharacterCustomizeRaces
        {
            itemSO = itemSO,
            skinColor = skinColor,
        };
    }
}

[Serializable]
public class CharacterCustomizeEyes : CharacterCustomize
{
    public Color irisColor = Color.black;
    public Color scleraColor = Color.white;
    public Color eyebrowEyelidColor = Color.black;

    public new CharacterCustomizeEyes Clone()
    {
        return new CharacterCustomizeEyes
        {
            itemSO = itemSO,
            irisColor = irisColor,
            scleraColor = scleraColor,
            eyebrowEyelidColor = eyebrowEyelidColor,
        };
    }
}

[Serializable]
public class CharacterCustomizeHair : CharacterCustomize
{
    public Color primaryColor = Color.black;
    public Color secondaryColor = Color.black;
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
}