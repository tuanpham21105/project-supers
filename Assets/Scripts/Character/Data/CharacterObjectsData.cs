using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectsData : MonoBehaviour
{
    // [Constant]
    [Header("Constant")]
    public GameObject characterObject;
    public Transform characterMesh;
    public GameObject characterHurtBox;

    [Header("Hair Accessories")]
    public GameObject frontHairMesh;
    public GameObject topHairMesh;
    public GameObject sideHairMesh;

    [Header("Head Accessories")]
    public GameObject headMesh;
    public GameObject eyesMesh;
    public GameObject mouthMesh;
    public GameObject faceAccessories;
    public GameObject headAccessories;

    [Header("Ears Accessories")]
    public GameObject earsMesh;
    public GameObject earsAccessories;

    [Header("Chest Accessories")]
    public GameObject neckAccessories;
    public GameObject backAccessories;
    public GameObject chestAccessories;
    public GameObject chestMesh;

    [Header("Hip Accessories")]
    public GameObject hipMesh;
    public GameObject hipAccessories;

    [Header("Right Forearm Accessories")]
    public GameObject rightHandAccessories;
    public GameObject rightForearmAccessories;
    public GameObject rightForearmMesh;

    [Header("Right Arm Accessories")]
    public GameObject rightShoulderAccessories;
    public GameObject rightArmMesh;

    [Header("Left Forearm Accessories")]
    public GameObject leftHandAccessories;
    public GameObject leftForearmAccessories;
    public GameObject leftForearmMesh;

    [Header("Left Arm Accessories")]
    public GameObject leftShoulderAccessories;
    public GameObject leftArmMesh;

    [Header("Right Shin Accessories")]
    public GameObject rightShinAccessories;
    public GameObject rightShinMesh;
    
    [Header("Right Thigh Accessories")]
    public GameObject rightThighAccessories;
    public GameObject rightThighMesh;

    [Header("Left Shin Accessories")]
    public GameObject leftShinAccessories;
    public GameObject leftShinMesh;
    
    [Header("Left Thigh Accessories")]
    public GameObject leftThighAccessories;
    public GameObject leftThighMesh;

    [Header("VFX")]
    public GameObject targetVfx;

    void Start()
    {
        if (characterObject == null) characterObject = gameObject;
        if (characterMesh == null) characterMesh = transform.Find("CharacterMesh");
        if (characterHurtBox == null) characterHurtBox = transform.Find("CharacterHurtBox").gameObject;
    }
}
