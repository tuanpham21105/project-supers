using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCustomizeHair", menuName = "Game/Character Customize/Hair")]
public class CharacterCustomizeHairSO : CharacterCustomizeItemSO
{
    public Mesh hair;
    public Material primaryMaterial;
    public Material secondaryMaterial;
    public Material tertiaryMaterial;
}
