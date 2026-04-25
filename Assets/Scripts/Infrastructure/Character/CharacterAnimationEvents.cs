using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public event System.Action OnNormalAttackOngoing;
    public event System.Action OnNormalAttackEndOngoing;
    public event System.Action OnNormalAttackEnd;
    public event System.Action OnStrikeAttackOngoing;
    public event System.Action OnStrikeAttackEndOngoing;
    public event System.Action OnStrikeAttackEnd;
    public event System.Action OnDeflectEnd;

    public void EmitNormalAttackOngoing()
    {
        OnNormalAttackOngoing?.Invoke();
    }

    public void EmitNormalAttackEndOngoing()
    {
        OnNormalAttackEndOngoing?.Invoke();
    }

    public void EmitNormalAttackEnd()
    {
        OnNormalAttackEnd?.Invoke();
    }

    public void EmitStrikeAttackOngoing()
    {
        OnStrikeAttackOngoing?.Invoke();
    }

    public void EmitStrikeAttackEndOngoing()
    {
        OnStrikeAttackEndOngoing?.Invoke();
    }

    public void EmitStrikeAttackEnd()
    {
        OnStrikeAttackEnd?.Invoke();
    }

    public void EmitDeflectEnd()
    {
        OnDeflectEnd?.Invoke();
    }
}
