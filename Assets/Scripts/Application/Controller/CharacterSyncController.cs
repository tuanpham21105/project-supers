using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum CharacterAnimationTypes
{
    upper,
    body,
    addition
}

public class CharacterSyncController : MonoBehaviour
{
    [Header("Dependencies")]
    private CharacterAnimationController characterAnimationController;

    void Start()
    {
        characterAnimationController = GetComponent<CharacterAnimationController>();
    }

    public void PlayAnimation(CharacterAnimationTypes animationType, String animationName)
    {
        switch (animationType)
        {
            case CharacterAnimationTypes.body:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "ground_idle";
                }
                if (Enum.TryParse(animationName, true, out CharacterBodyAnimation bodyAnim))
                {
                    characterAnimationController.PlayBodyAnimation(bodyAnim);
                }
                break;

            case CharacterAnimationTypes.upper:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "none";
                    characterAnimationController.EndUpperAnimation();
                }
                if (Enum.TryParse(animationName, true, out CharacterUpperAnimation upperAnim))
                {
                    if (upperAnim == CharacterUpperAnimation.none)
                        characterAnimationController.EndUpperAnimation();
                    characterAnimationController.PlayUpperAnimation(upperAnim);
                }
                break;

            case CharacterAnimationTypes.addition:
                if (string.IsNullOrEmpty(animationName))
                {
                    animationName = "none";
                }
                if (Enum.TryParse(animationName, true, out AdditionalAnimation addAnim))
                {
                    characterAnimationController.PlayAdditionalAnimation(addAnim);
                }
                break;
        }
    }
   
    public void ApplyTransform(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ApplyPhysicsCollider(float radius, float height)
    {
        GetComponent<CharacterController>().radius = radius;
        GetComponent<CharacterController>().height = height;
    }
}
