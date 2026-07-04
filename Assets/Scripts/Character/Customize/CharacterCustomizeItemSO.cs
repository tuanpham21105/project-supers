using UnityEngine;

// [CreateAssetMenu(fileName = "CharacterCustomizeItem", menuName = "Game/Character Customize Item")]
public class CharacterCustomizeItemSO : ScriptableObject
{
    public string code;
    public string itemName;
    public Sprite itemSprite;
    public CharacterCustomizeType type;
}
