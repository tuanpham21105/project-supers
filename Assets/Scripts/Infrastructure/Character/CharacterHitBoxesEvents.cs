using System;
using UnityEngine;

public class CharacterHitBoxesEvents : MonoBehaviour
{
    public event Action<GameObject> OnNormalAttack;
    public event Action<GameObject> OnStrikeAttack;
    public event Action OnAttackInterrupt;

    public void EmitNormalAttack(GameObject target)
    {
        OnNormalAttack?.Invoke(target);
    }

    public void EmitStrikeAttack(GameObject target)
    {
        OnStrikeAttack?.Invoke(target);
    }

    public void EmitAttackInterrupt()
    {
        OnAttackInterrupt?.Invoke();
    }
}
