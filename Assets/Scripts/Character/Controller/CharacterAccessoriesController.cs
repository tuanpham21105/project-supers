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
            AccessoriesPart.Chest => characterObjectsData.chestAccessories,
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
            AccessoriesPiecePart.Chest => characterObjectsData.chestAccessories,
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
            AccessoriesPiecePart.FrontHair => characterObjectsData.frontHairMesh,
            AccessoriesPiecePart.TopHair => characterObjectsData.topHairMesh,
            AccessoriesPiecePart.SideHair => characterObjectsData.sideHairMesh,
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

    //============================================================
    public void SetCharacterCustomize(CharacterCustomizeItemSO itemSO)
    {
        switch (itemSO.type)
        {
            case CharacterCustomizeType.Races:
                SetCharacterRaces(itemSO);
                break;
            case CharacterCustomizeType.Eyes:
                SetCharacterEyes(itemSO);
                break;
            case CharacterCustomizeType.Mouth:
                SetCharacterMouth(itemSO);
                break;
            case CharacterCustomizeType.Front_Hair:
                SetCharacterHair(AccessoriesPiecePart.FrontHair, itemSO);
                break;
            case CharacterCustomizeType.Top_Hair:
                SetCharacterHair(AccessoriesPiecePart.TopHair, itemSO);
                break;
            case CharacterCustomizeType.Side_Hair:
                SetCharacterHair(AccessoriesPiecePart.SideHair, itemSO);
                break;
        }
    }

    [ProButton]
    void SetCharacterRaces(CharacterCustomizeItemSO itemSO)
    {
        CharacterCustomizeRacesSO racesSO = itemSO as CharacterCustomizeRacesSO;

        characterAccessoriesData.racesItemSO = racesSO;

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
        target.GetComponent<MeshFilter>().sharedMesh = origin;
    }

    void SetMaterial(Material original, GameObject target)
    {
        target.GetComponent<MeshRenderer>().sharedMaterial = original;
    }

    void SetCharacterEyes(CharacterCustomizeItemSO itemSO)
    {
        CharacterCustomizeEyesSO eyesSO = itemSO as CharacterCustomizeEyesSO;

        MeshRenderer renderer = characterObjectsData.eyesMesh.GetComponent<MeshRenderer>();

        Material[] materials = renderer.sharedMaterials;

        materials[2] = eyesSO.eyelidMaterial;
        materials[1] = eyesSO.scleraMaterial;
        materials[0] = eyesSO.irisMaterial;
        // materials[0] = eyesSO.pupilMaterial;

        renderer.sharedMaterials = materials;
    }

    void SetCharacterMouth(CharacterCustomizeItemSO itemSO)
    {
        CharacterCustomizeMouthSO mouthSO = itemSO as CharacterCustomizeMouthSO;

        SetMaterial(mouthSO.mouthMaterial, characterObjectsData.mouthMesh);
    }

    void SetCharacterHair(AccessoriesPiecePart part, CharacterCustomizeItemSO itemSO)
    {
        CharacterCustomizeHairSO hairSO = itemSO as CharacterCustomizeHairSO;

        GameObject hairMesh;

        switch (part)
        {
            case AccessoriesPiecePart.FrontHair:
                hairMesh = characterObjectsData.frontHairMesh;
                break;
            case AccessoriesPiecePart.TopHair:
                hairMesh = characterObjectsData.topHairMesh;
                break;
            default:
                hairMesh = characterObjectsData.sideHairMesh;
                break;
        }

        MeshRenderer renderer = hairMesh.GetComponent<MeshRenderer>();

        Material[] materials = renderer.sharedMaterials;

        materials[0] = hairSO.primaryMaterial;
        materials[1] = hairSO.secondaryMaterial;
        materials[2] = hairSO.tertiaryMaterial;

        renderer.sharedMaterials = materials;

        SetMesh(hairSO.hair, hairMesh);
    }

    public void SetRacesColor(Color skinColor)
    {
        SetColorInternal(characterObjectsData.headMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.earsMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.chestMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.hipMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.rightForearmMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.rightArmMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.leftForearmMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.leftArmMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.rightShinMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.rightThighMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.leftShinMesh.GetComponent<Renderer>(), 0, skinColor);
        SetColorInternal(characterObjectsData.leftThighMesh.GetComponent<Renderer>(), 0, skinColor);
    }

    public void SetEyesColors(Color irisColor, Color scleraColor, Color eyelidColor)
    {
        Renderer renderer = characterObjectsData.eyesMesh.GetComponent<Renderer>();
        SetColorInternal(renderer, 0, irisColor);
        SetColorInternal(renderer, 1, scleraColor);
        SetColorInternal(renderer, 2, eyelidColor);
    }

    public void SetHairColors(Color primaryColor, Color secondaryColor, Color tertiaryColor)
    {
        Renderer fRenderer = characterObjectsData.frontHairMesh.GetComponent<Renderer>();
        SetColorInternal(fRenderer, 0, primaryColor);    
        SetColorInternal(fRenderer, 1, secondaryColor);    
        SetColorInternal(fRenderer, 2, tertiaryColor);    

        Renderer tRenderer = characterObjectsData.topHairMesh.GetComponent<Renderer>();
        SetColorInternal(tRenderer, 0, primaryColor);    
        SetColorInternal(tRenderer, 1, secondaryColor);    
        SetColorInternal(tRenderer, 2, tertiaryColor);  

        Renderer sRenderer = characterObjectsData.sideHairMesh.GetComponent<Renderer>();
        SetColorInternal(sRenderer, 0, primaryColor);    
        SetColorInternal(sRenderer, 1, secondaryColor);    
        SetColorInternal(sRenderer, 2, tertiaryColor);  
    }

    private MaterialPropertyBlock block;
    private void SetColorInternal(Renderer renderer, int materialIndex, Color color)
    {
        if (renderer.sharedMaterials.Length <= materialIndex)
            return;

        if (block == null)
            block = new MaterialPropertyBlock();
            
        renderer.GetPropertyBlock(block, materialIndex);
        block.SetColor("_Color", color);
        block.SetColor("baseColorFactor", color);
        renderer.SetPropertyBlock(block, materialIndex);
    }

    //
    public void SetEmblem(Material material)
    {
        if (characterObjectsData.emblemMaterial != null)
        {
            // Lấy texture đang gán trong material RA TRƯỚC khi destroy material
            Texture tex = characterObjectsData.emblemMaterial.GetTexture("_ShadowTex");

            Destroy(characterObjectsData.emblemMaterial);
            characterObjectsData.emblemMaterial = null;

            // Destroy texture lấy được, ép kiểu về RenderTexture để gọi Release()
            if (tex is RenderTexture rt)
            {
                rt.Release();
                Destroy(rt);
            }
        }

        characterObjectsData.emblemMaterial = material;
        characterObjectsData.emblemProjector.material = material;
    }
}