using System;
using System.Collections.Generic;
using UnityEngine;

public enum AccessoriesPiecePart
{
    Head,
    Mask,
    Ears,
    Neck,
    Chest,
    Back,
    Hip,
    RightArm,
    RightForearm,
    RightHand,
    LeftArm,
    LeftForearm,
    LeftHand,
    RightThigh,
    RightShin,
    LeftThigh,
    LeftShin
}

public enum AccessoriesType
{
    Override,
    Additive
}

public enum AccessoriesPart
{
    Hat,
    Mask,
    Neck,
    Chest,
    Back,
    Shoulders,
    Gloves,
    Hip,
    Leg,
    Boots
}

[Serializable]
public class AccessoryPiece
{
    public GameObject gameObject;
    public AccessoriesPiecePart piece;
    public AccessoriesType type;

}

public class CharacterAccessoryItemData : MonoBehaviour
{
    public AccessoriesPart part;
    public List<AccessoryPiece> pieces;
}
