using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeTypeButtonUiController : MonoBehaviour
{
    [SerializeField] private CharacterCustomizeType type;

    public event Action<CharacterCustomizeType> onClick;

    public void Click()
    {
        onClick?.Invoke(type);
    }
}
