using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizeItemsList", menuName = "Game/Character Customize Items List")]
public class CharacterCustomizeItemsListSO : ScriptableObject
{
    public List<CharacterCustomizeItemSO> items = new List<CharacterCustomizeItemSO>();

    public CharacterCustomizeItemSO findByCode(string code)
    {
        if (code == "" || code == null)
            return items[0];

        foreach (CharacterCustomizeItemSO item in items) 
        {
            if (item.code.Equals(code)) 
                return item;
        }

        return items[0];
    }
}