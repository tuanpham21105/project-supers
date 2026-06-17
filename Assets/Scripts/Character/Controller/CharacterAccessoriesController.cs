using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class CharacterAccessoriesController : MonoBehaviour
{
    [Header("Dependencies")]
    private CharacterAccessoriesData characterAccessoriesData;
    private CharacterObjectsData characterObjectsData;

    void Start()
    {
        if (characterAccessoriesData == null) characterAccessoriesData = GetComponent<CharacterAccessoriesData>();
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
    }

    [ProButton]
    public void PutOnAccessories(CharacterAccessoriesItem item)
    {
        if (item == null) return;

        AccessoriesPart part = item.part;

        CharacterAccessoriesItem existingItem = GetAccessoriesSlot(part);
        if (existingItem != null)
        {
            TakeOffAccessories(existingItem);
        }

        SetAccessoriesSlot(part, item);

        GameObject slotObject = GetAccessoriesSlotObject(part);
        if (slotObject != null)
        {
            item.transform.SetParent(slotObject.transform, false);
        }

        foreach (AccessoryPiece piece in item.pieces)
        {
            if (piece.gameObject == null) continue;

            GameObject pieceSlot = GetPieceSlotObject(piece.piece);
            if (pieceSlot != null)
            {
                piece.gameObject.transform.SetParent(pieceSlot.transform, false);
            }

            if (piece.type == AccessoriesType.Override)
            {
                GameObject partMesh = GetPartMesh(piece.piece);
                if (partMesh != null)
                {
                    partMesh.SetActive(false);
                }
            }
        }
    }

    [ProButton]
    public void TakeOffAccessories(CharacterAccessoriesItem item)
    {
        if (item == null) return;

        foreach (AccessoryPiece piece in item.pieces)
        {
            if (piece.gameObject == null) continue;

            if (piece.type == AccessoriesType.Override)
            {
                GameObject partMesh = GetPartMesh(piece.piece);
                if (partMesh != null)
                {
                    partMesh.SetActive(true);
                }
            }

            Destroy(piece.gameObject);
        }

        SetAccessoriesSlot(item.part, null);

        Destroy(item.gameObject);
    }

    private CharacterAccessoriesItem GetAccessoriesSlot(AccessoriesPart part)
    {
        return part switch
        {
            AccessoriesPart.Hat => characterAccessoriesData.hatAccessories,
            AccessoriesPart.Mask => characterAccessoriesData.maskAccessories,
            AccessoriesPart.Neck => characterAccessoriesData.neckAccessories,
            AccessoriesPart.Chest => characterAccessoriesData.chestAccessories,
            AccessoriesPart.Back => characterAccessoriesData.backAccessories,
            AccessoriesPart.Shoulders => characterAccessoriesData.shouldersAccessories,
            AccessoriesPart.Gloves => characterAccessoriesData.glovesAccessories,
            AccessoriesPart.Hip => characterAccessoriesData.hipAccessories,
            AccessoriesPart.Leg => characterAccessoriesData.legAccessories,
            AccessoriesPart.Boots => characterAccessoriesData.bootsAccessories,
            _ => null
        };
    }

    private void SetAccessoriesSlot(AccessoriesPart part, CharacterAccessoriesItem item)
    {
        switch (part)
        {
            case AccessoriesPart.Hat: characterAccessoriesData.hatAccessories = item; break;
            case AccessoriesPart.Mask: characterAccessoriesData.maskAccessories = item; break;
            case AccessoriesPart.Neck: characterAccessoriesData.neckAccessories = item; break;
            case AccessoriesPart.Chest: characterAccessoriesData.chestAccessories = item; break;
            case AccessoriesPart.Back: characterAccessoriesData.backAccessories = item; break;
            case AccessoriesPart.Shoulders: characterAccessoriesData.shouldersAccessories = item; break;
            case AccessoriesPart.Gloves: characterAccessoriesData.glovesAccessories = item; break;
            case AccessoriesPart.Hip: characterAccessoriesData.hipAccessories = item; break;
            case AccessoriesPart.Leg: characterAccessoriesData.legAccessories = item; break;
            case AccessoriesPart.Boots: characterAccessoriesData.bootsAccessories = item; break;
        }
    }

    private GameObject GetAccessoriesSlotObject(AccessoriesPart part)
    {
        return part switch
        {
            AccessoriesPart.Hat => characterObjectsData.headAccessories,
            AccessoriesPart.Mask => characterObjectsData.faceAccessories,
            AccessoriesPart.Neck => characterObjectsData.neckAccessories,
            AccessoriesPart.Chest => characterObjectsData.backAccessories,
            AccessoriesPart.Back => characterObjectsData.backAccessories,
            AccessoriesPart.Shoulders => characterObjectsData.rightShoulderAccessories,
            AccessoriesPart.Gloves => characterObjectsData.rightHandAccessories,
            AccessoriesPart.Hip => characterObjectsData.hipAccessories,
            AccessoriesPart.Leg => characterObjectsData.rightThighAccessories,
            AccessoriesPart.Boots => characterObjectsData.rightShinAccessories,
            _ => null
        };
    }

    private GameObject GetPieceSlotObject(AccessoriesPiecePart part)
    {
        return part switch
        {
            AccessoriesPiecePart.Head => characterObjectsData.headAccessories,
            AccessoriesPiecePart.Mask => characterObjectsData.faceAccessories,
            AccessoriesPiecePart.Ears => characterObjectsData.earsAccessories,
            AccessoriesPiecePart.Neck => characterObjectsData.neckAccessories,
            AccessoriesPiecePart.Chest => characterObjectsData.backAccessories,
            AccessoriesPiecePart.Back => characterObjectsData.backAccessories,
            AccessoriesPiecePart.Hip => characterObjectsData.hipAccessories,
            AccessoriesPiecePart.RightArm => characterObjectsData.rightShoulderAccessories,
            AccessoriesPiecePart.RightForearm => characterObjectsData.rightForearmAccessories,
            AccessoriesPiecePart.RightHand => characterObjectsData.rightHandAccessories,
            AccessoriesPiecePart.LeftArm => characterObjectsData.leftShoulderAccessories,
            AccessoriesPiecePart.LeftForearm => characterObjectsData.leftForearmAccessories,
            AccessoriesPiecePart.LeftHand => characterObjectsData.leftHandAccessories,
            AccessoriesPiecePart.RightThigh => characterObjectsData.rightThighAccessories,
            AccessoriesPiecePart.RightShin => characterObjectsData.rightShinAccessories,
            AccessoriesPiecePart.LeftThigh => characterObjectsData.leftThighAccessories,
            AccessoriesPiecePart.LeftShin => characterObjectsData.leftShinAccessories,
            _ => null
        };
    }

    private GameObject GetPartMesh(AccessoriesPiecePart part)
    {
        return part switch
        {
            AccessoriesPiecePart.Head => characterObjectsData.headMesh,
            AccessoriesPiecePart.Ears => characterObjectsData.earsMesh,
            AccessoriesPiecePart.Chest => characterObjectsData.chestMesh,
            AccessoriesPiecePart.Hip => characterObjectsData.hipMesh,
            AccessoriesPiecePart.RightArm => characterObjectsData.rightArmMesh,
            AccessoriesPiecePart.RightForearm => characterObjectsData.rightForearmMesh,
            AccessoriesPiecePart.LeftArm => characterObjectsData.leftArmMesh,
            AccessoriesPiecePart.LeftForearm => characterObjectsData.leftForearmMesh,
            AccessoriesPiecePart.RightThigh => characterObjectsData.rightThighMesh,
            AccessoriesPiecePart.RightShin => characterObjectsData.rightShinMesh,
            AccessoriesPiecePart.LeftThigh => characterObjectsData.leftThighMesh,
            AccessoriesPiecePart.LeftShin => characterObjectsData.leftShinMesh,
            _ => null
        };
    }
}