using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAccessoriesController : MonoBehaviour
{
    [Header("Dependencies")]
    private CharacterAccessoriesData characterAccessoriesData;
    private CharacterObjectsData characterObjectsData;

    void Awake()
    {
        if (characterAccessoriesData == null) characterAccessoriesData = GetComponent<CharacterAccessoriesData>();
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
    }

    [ProButton]
    public void PutOn(AccessoryItemSO item, AccessoryProperties properties)
    {
        if (item == null) return;

        AccessoriesPart part = item.part;

        TakeOff(part);

        if (item.itemPrefab == null) return;

        GameObject newItem = Instantiate(item.itemPrefab, transform);
        CharacterAccessoryItemData newItemData = newItem.GetComponent<CharacterAccessoryItemData>();
        if (newItemData == null) return;

        SetAccessoriesSlot(part, newItemData);

        GameObject slotObject = GetAccessoriesSlotObject(part);
        if (slotObject != null)
        {
            newItemData.transform.SetParent(slotObject.transform, false);
        }

        foreach (AccessoryPiece piece in newItemData.pieces)
        {
            if (piece == null) continue;

            if (piece.gameObject != null)
            {
                GameObject pieceSlot = GetPieceSlotObject(piece.piece);
                if (pieceSlot != null)
                {
                    piece.gameObject.transform.SetParent(pieceSlot.transform, false);
                    piece.gameObject.transform.localPosition = Vector3.zero;
                }
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

        newItemData.SetColors(properties);

        newItemData.character = gameObject;
    }

    [ProButton]
    public void TakeOff(AccessoriesPart part)
    {
        CharacterAccessoryItemData existingItem = GetAccessoriesSlot(part);
        if (existingItem == null) return;

        foreach (AccessoryPiece piece in existingItem.pieces)
        {
            if (piece == null) continue;

            if (piece.type == AccessoriesType.Override)
            {
                GameObject partMesh = GetPartMesh(piece.piece);
                if (partMesh != null)
                {
                    partMesh.SetActive(true);
                }
            }

            if (piece.gameObject != null)
            {
                Destroy(piece.gameObject);
            }
        }

        SetAccessoriesSlot(part, null);

        Destroy(existingItem.gameObject);
    }

    private CharacterAccessoryItemData GetAccessoriesSlot(AccessoriesPart part)
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

    private void SetAccessoriesSlot(AccessoriesPart part, CharacterAccessoryItemData item)
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

    public void SetAccessoryColorsByPart(AccessoryProperties properties, AccessoriesPart part)
    {
        CharacterAccessoryItemData slot = GetAccessoriesSlot(part);
        if (slot == null) return;

        slot.SetColors(properties);
    }

    //
    public void SetCharacterCustomize(CharacterCustomizeItemSO itemSO)
    {
        switch (itemSO.type)
        {
            case CharacterCustomizeType.Races:
                CharacterCustomizeRacesSO racesSO = itemSO as CharacterCustomizeRacesSO;
                SetCharacterRaces(racesSO);
                break;
            case CharacterCustomizeType.Eyes:
                break;
            case CharacterCustomizeType.Mouth:
                break;
            case CharacterCustomizeType.Front_Hair:
                break;
            case CharacterCustomizeType.Top_Hair:
                break;
            case CharacterCustomizeType.Side_Hair:
                break;
        }
    }

    [ProButton]
    void SetCharacterRaces(CharacterCustomizeRacesSO racesSO)
    {
        SetMesh(racesSO.headMesh, characterObjectsData.headMesh);
        SetMesh(racesSO.earsMesh, characterObjectsData.earsMesh);
        SetMesh(racesSO.chestMesh, characterObjectsData.chestMesh);
        SetMesh(racesSO.hipMesh, characterObjectsData.hipMesh);
        SetMesh(racesSO.rightForearmMesh, characterObjectsData.rightForearmMesh);
        SetMesh(racesSO.rightArmMesh, characterObjectsData.rightArmMesh);
        SetMesh(racesSO.leftForearmMesh, characterObjectsData.leftForearmMesh);
        SetMesh(racesSO.leftArmMesh, characterObjectsData.leftArmMesh);
        SetMesh(racesSO.rightShinMesh, characterObjectsData.rightShinMesh);
        SetMesh(racesSO.rightThighMesh, characterObjectsData.rightThighMesh);
        SetMesh(racesSO.leftShinMesh, characterObjectsData.leftShinMesh);
        SetMesh(racesSO.leftThighMesh, characterObjectsData.leftThighMesh);
    }

    void SetMesh(Mesh origin, GameObject target)
    {
        target.GetComponent<MeshFilter>().mesh = origin;
    }


}