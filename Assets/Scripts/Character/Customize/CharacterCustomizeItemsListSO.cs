using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizeItem", menuName = "Game/Character Customize Item")]
public class CharacterCustomizeItemsListSO : ScriptableObject
{
    public List<CharacterCustomizeItemSo> items = new List<CharacterCustomizeItemSo>();

    public CharacterCustomizeItemSo findByCode(string code)
    {
        if (code == "" || code == null)
            return items[0];

        foreach (CharacterCustomizeItemSo item in items) 
        {
            if (item.code.Equals(code)) 
                return item;
        }

        return items[0];
    }
}