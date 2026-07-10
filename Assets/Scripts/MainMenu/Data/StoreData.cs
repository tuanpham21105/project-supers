using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreData : MonoBehaviour
{
    public static StoreData instance;

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

    [Header("Accessories Lists")]
    [SerializeField] private AccessoriesListSO localHatList;
    [SerializeField] private AccessoriesListSO localMaskList;
    [SerializeField] private AccessoriesListSO localNeckList;
    [SerializeField] private AccessoriesListSO localChestList;
    [SerializeField] private AccessoriesListSO localBackList;
    [SerializeField] private AccessoriesListSO localShouldersList;
    [SerializeField] private AccessoriesListSO localGlovesList;
    [SerializeField] private AccessoriesListSO localHipList;
    [SerializeField] private AccessoriesListSO localLegList;
    [SerializeField] private AccessoriesListSO localBootsList;

    public AccessoriesListSO GetLocalListByType(StoreItemsType type)
    {
        return type switch
        {
            StoreItemsType.Hat => localHatList,
            StoreItemsType.Mask => localMaskList,
            StoreItemsType.Neck => localNeckList,
            StoreItemsType.Chest => localChestList,
            StoreItemsType.Back => localBackList,
            StoreItemsType.Shoulders => localShouldersList,
            StoreItemsType.Gloves => localGlovesList,
            StoreItemsType.Hip => localHipList,
            StoreItemsType.Leg => localLegList,
            StoreItemsType.Boots => localBootsList,
            _ => null
        };
    }

    [Header("Customize Lists")]
    [SerializeField] private CharacterCustomizeItemsListSO racesList;
    [SerializeField] private CharacterCustomizeItemsListSO eyesList;
    [SerializeField] private CharacterCustomizeItemsListSO mouthList;
    [SerializeField] private CharacterCustomizeItemsListSO frontHairList;
    [SerializeField] private CharacterCustomizeItemsListSO topHairList;
    [SerializeField] private CharacterCustomizeItemsListSO sideHairList;

    public CharacterCustomizeItemsListSO GetCustomizeListByType(CharacterCustomizeType type)
    {
        switch (type)
        {
            case CharacterCustomizeType.Races:
                return racesList;
            case CharacterCustomizeType.Eyes:
                return eyesList;
            case CharacterCustomizeType.Mouth:
                return mouthList;
            case CharacterCustomizeType.Front_Hair:
                return frontHairList;
            case CharacterCustomizeType.Top_Hair:
                return topHairList;
            case CharacterCustomizeType.Side_Hair:
                return sideHairList;
            default:
                return null;
        }
    }

    [SerializeField] public List<Shape> shapesList = new List<Shape>();
}
