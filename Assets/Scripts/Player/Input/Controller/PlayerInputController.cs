using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController instance;

    public abstract void OpenSettingInput();
    public abstract void HandleCloseSetting();
    public abstract void HandleFlyingInterrupted();
    public abstract void MoveDirectionInput();
    public abstract void SprintInput();
    public abstract void DashInput();
    public abstract void ToggleFlyInput();
    public abstract void JumpInput();
    public abstract void FlyUpInput();
    public abstract void FlyDownInput();
    public abstract void NormalAttackInput();
    public abstract void StrikeAttackInput();
    public abstract void RotationInput();
    public abstract void BlockInput();
    public abstract void DeflectInput();
}

