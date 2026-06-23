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
}
