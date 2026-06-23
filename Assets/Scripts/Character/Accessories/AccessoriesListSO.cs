using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AccessoriesList", menuName = "Game/Accessories List")]
public class AccessoriesListSO : ScriptableObject
{
    public List<AccessoryItemSO> items = new List<AccessoryItemSO>();

    public AccessoryItemSO findByCode(String code)
    {
        foreach (AccessoryItemSO item in items) 
        {
            if (item.code.Equals(code)) 
                return item;
        }

        return null;
    }
}