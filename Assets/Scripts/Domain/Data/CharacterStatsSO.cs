using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStatsData", menuName = "Game/Character Stats Data")]
public class CharacterStatsSO : ScriptableObject
{
    public float moveSpeed;
    public float sprintAdditionalSpeed;
    public float flySpeed;
    public float flySprintAdditionalSpeed;
    public float jumpForce;
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public int endurance;
    public float knockOutThreshold;
    public float blockThreshold;
    public int normalAttackDamage;
    public int strikeAttackDamage;
    public float damageKnockAwayRatio;
    public float maxCombineAttackAngleSize;
    public float sqrMoveSpeedDamageThreshold;
    public float continueAttackWindow;
    public float deflectComboWindow;
}
