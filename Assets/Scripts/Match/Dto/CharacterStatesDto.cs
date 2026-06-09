using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vec3
{
    public float x, y, z;

    public static Vec3 From(Vector3 v) => new Vec3 { x = v.x, y = v.y, z = v.z };
    public Vector3 ToVector3() => new Vector3(x, y, z);
    public Vector2 ToVector2() => new Vector2(x, y);
}

[Serializable]
public struct Quat
{
    public float x, y, z, w;

    public static Quat From(Quaternion q) => new Quat { x = q.x, y = q.y, z = q.z, w = q.w };
    public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
}

public class CharacterStatesDto
{
    public Vec3  position;
    public Vec3  forward;
    public float physicsColliderRadius;
    public float physicsColliderHeight;

    public String currentProcessAction;

    public bool moveFlag;
    public bool jumpFlag;
    public bool sprintFlag;
    public bool dashFlag;
    public bool dashCooldownFlag;
    public bool flyFlag;
    public bool flyUpFlag;
    public bool flyDownFlag;
    public bool fastFlyFlag;
    public bool attackFlag;
    public bool normalAttackStartFlag;
    public bool normalAttackOngoingFlag;
    public bool normalAttackEndFlag;
    public bool strikeAttackStartFlag;
    public bool strikeAttackOngoingFlag;
    public bool strikeAttackEndFlag;
    public bool knockAwayFlag;
    public bool blockFlag;
    public bool deflectFlag;
    public bool upperActionFlag;
    public bool bodyActionFlag;
    public bool hitFlag;
    public bool deflectedFlag;
    public bool deadFlag;
    public bool fallFlag;

    public int currentEndurance;
    public float moveSpeed;

    public Vec3 inputAxes;
    public Vec3 lookInput;
    public Vec3 direction;

    public String currentBodyAnimation;

    public float lastNormalAttackEndTime = -Mathf.Infinity;
    public float lastStrikeAttackEndTime = -Mathf.Infinity;

    public float lastDeflectTime;
    public float currentDeflectSpeed = 1f;

    public int normalAttackComboIndex = 0;
    public int strikeAttackComboIndex = 0;
    public int hitAnimationIndex = 0;

    public float verticalVelocity;
    public Vec3 impactForce;
    public Vec3 dashForce;
    public float dashTimer;
    public Vec3 horizontalMove;
    public bool isImpactActive;
    public Vec3 currentMoveDirection;
    public float currentSqrMoveSpeed;

    public bool isFront;
}
