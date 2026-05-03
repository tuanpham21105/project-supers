using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsData : MonoBehaviour
{
    // [Constant]
    [Header("Constant")]
    [SerializeField] private CharacterStatsSO characterStatsSO;
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

    void Start()
    {
        if (characterStatsSO == null)
        {
            Debug.LogWarning("CharacterStatsSO is not assigned in CharacterStatsData!");
            return;
        }

        moveSpeed = characterStatsSO.moveSpeed;
        sprintAdditionalSpeed = characterStatsSO.sprintAdditionalSpeed;
        flySpeed = characterStatsSO.flySpeed;
        flySprintAdditionalSpeed = characterStatsSO.flySprintAdditionalSpeed;
        jumpForce = characterStatsSO.jumpForce;
        dashForce = characterStatsSO.dashForce;
        dashDuration = characterStatsSO.dashDuration;
        dashCooldown = characterStatsSO.dashCooldown;
        endurance = characterStatsSO.endurance;
        knockOutThreshold = characterStatsSO.knockOutThreshold;
        blockThreshold = characterStatsSO.blockThreshold;
        normalAttackDamage = characterStatsSO.normalAttackDamage;
        strikeAttackDamage = characterStatsSO.strikeAttackDamage;
        damageKnockAwayRatio = characterStatsSO.damageKnockAwayRatio;
        maxCombineAttackAngleSize = characterStatsSO.maxCombineAttackAngleSize;
        sqrMoveSpeedDamageThreshold = characterStatsSO.sqrMoveSpeedDamageThreshold;
        continueAttackWindow = characterStatsSO.continueAttackWindow;
        deflectComboWindow = characterStatsSO.deflectComboWindow;
    }
}
