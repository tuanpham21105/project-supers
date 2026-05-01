using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHurtBoxService : MonoBehaviour
{
    [SerializeField] private GameObject character;

    public GameObject GetCharacter() => character;

    void Start()
    {
        if (character == null) character = transform.parent.gameObject;
    }

    public void RotateLocal(Vector3 target)
    {
        transform.localRotation = Quaternion.Euler(target);
    }
}
