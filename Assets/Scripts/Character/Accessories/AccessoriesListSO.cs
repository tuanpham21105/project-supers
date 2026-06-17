using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AccessoriesList", menuName = "Game/Accessories List")]
public class AccessoriesListSO : ScriptableObject
{
    public List<AccessoryItemSO> items;
}