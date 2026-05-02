using System;
using UnityEngine;

public class CharacterHitBoxesEvents : MonoBehaviour
{
    // [Event]
    public event Action<GameObject, AttackTypes> OnAttackHit;
    public event Action OnAttackInterrupt;
    public event Action OnStartFlyAttack;
    public event Action OnEndFlyAttack;

    public void EmitAttackHit(GameObject target, AttackTypes type)
    {
        OnAttackHit?.Invoke(target, type);
    }

    public void EmitAttackInterrupt()
    {
        OnAttackInterrupt?.Invoke();
    }

    public void EmitStartFlyAttack()
    {
        OnStartFlyAttack?.Invoke();
    }

    public void EmitEndFlyAttack()
    {
        OnEndFlyAttack?.Invoke();
    }
}
