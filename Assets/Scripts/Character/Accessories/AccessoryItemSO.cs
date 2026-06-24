using UnityEngine;

[CreateAssetMenu(fileName = "AccessoryItem", menuName = "Game/Accessory Item")]
public class AccessoryItemSO : ScriptableObject
{
    public string code;
    public string itemName;
    public AccessoriesPart part;
    public Sprite image;
    public GameObject itemPrefab;
}