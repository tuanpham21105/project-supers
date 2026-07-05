using System;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
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
    LeftShin,
    FrontHair,
    TopHair,
    SideHair
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
    public List<AccessoryPiece> pieces;

    private MaterialPropertyBlock block;

    public GameObject character {get; set;}

    [ProButton]
    public void InspectorSetColors(Color p, Color s, Color t)
    {
        SetColors(new AccessoryProperties()
            {
                primaryColor = p, 
                secondaryColor = s, 
                tertiaryColor = t
            }
        );
    }

    public void SetColors(AccessoryProperties properties)
    {
        if (block == null)
            block = new MaterialPropertyBlock();

        foreach (var piece in pieces)
        {
            if (piece == null || piece.gameObject == null) continue;

            Renderer renderer = piece.gameObject.GetComponent<Renderer>();
            if (renderer == null) continue;

            if (renderer.sharedMaterials.Length > 0)
                SetColorInternal(renderer, 0, properties.primaryColor);
            if (renderer.sharedMaterials.Length > 1)
                SetColorInternal(renderer, 1, properties.secondaryColor);
            if (renderer.sharedMaterials.Length > 2)
                SetColorInternal(renderer, 2, properties.tertiaryColor);
        }
    }

    private void SetColorInternal(Renderer renderer, int materialIndex, Color color)
    {
        if (renderer.sharedMaterials.Length <= materialIndex)
            return;

        renderer.GetPropertyBlock(block, materialIndex);
        block.SetColor("_Color", color);
        block.SetColor("baseColorFactor", color);
        renderer.SetPropertyBlock(block, materialIndex);
    }
}
