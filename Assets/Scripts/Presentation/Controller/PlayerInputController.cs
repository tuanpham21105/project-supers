using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController instance;
    
    public abstract void MoveDirectionInput();
    public abstract void SprintInput();
    public abstract void DashInput();
    public abstract void ToggleFlyInput();
    public abstract void JumpInput();
    public abstract void FlyUpInput();
    public abstract void FlyDownInput();
    public abstract void RotationInput();
}

