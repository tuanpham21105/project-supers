using UnityEngine;

public enum CharacterCustomizeType
{
    Races,
    Eyes,
    Mouth,
    Front_Hair,
    Top_Hair,
    Side_Hair
}

[CreateAssetMenu(fileName = "CharacterCustomizeItem", menuName = "Game/Character Customize Item")]
public class CharacterCustomizeItemSo : ScriptableObject
{
    public string code;
    public string itemName;
    public Sprite itemSprite;
    public CharacterCustomizeType type;
    public GameObject prefab;
}