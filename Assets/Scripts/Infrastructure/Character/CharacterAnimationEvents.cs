using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEvents : MonoBehaviour
{
    public event System.Action OnNormalAttackOngoing;
    public event System.Action OnNormalAttackEndOngoing;
    public event System.Action OnNormalAttackEnd;

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
}
