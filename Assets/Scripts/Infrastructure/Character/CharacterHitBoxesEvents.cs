using System;
using UnityEngine;

public class CharacterHitBoxesEvents : MonoBehaviour
{
    public event Action<GameObject> OnNormalAttack;
    public event Action<GameObject> OnStrikeAttack;

    public void EmitNormalAttack(GameObject target)
    {
        OnNormalAttack?.Invoke(target);
    }

    public void EmitStrikeAttack(GameObject target)
    {
        OnStrikeAttack?.Invoke(target);
    }
}
