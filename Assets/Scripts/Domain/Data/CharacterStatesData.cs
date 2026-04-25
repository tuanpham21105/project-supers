using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatesData : MonoBehaviour
{
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

    public float moveSpeed;
    public Vector2 inputAxes;
    public Vector3 lookInput;
    public float horizontalRotation;
    public float verticalRotation;
    public Vector3 direction;
}
