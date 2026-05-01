using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterObjectsData : MonoBehaviour
{
    public GameObject characterObject;
    public Transform characterMesh;
    public GameObject characterHurtBox;

    void Start()
    {
        if (characterObject == null) characterObject = gameObject;
        if (characterMesh == null) characterMesh = transform.Find("CharacterMesh");
        if (characterHurtBox == null) characterHurtBox = transform.Find("CharacterHurtBox").gameObject;
    }
}
