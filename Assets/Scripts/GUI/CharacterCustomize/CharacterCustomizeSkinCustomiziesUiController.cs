using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeSkinCustomiziesUiController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private ColorPickerUiController colorPickerUiController;
    [SerializeField] private ColorLevelButtonUiController skinColorButton;

    //
    public event Action<Color> onPickColor;

    public void PickColor(Color color)
    {
        onPickColor?.Invoke(color);

        SetColor(color);
    }

    public void SetColor(Color skin)
    {
        skinColorButton.SetColor(skin);
    }

    void OnEnable()
    {
        colorPickerUiController.onPickColor += PickColor;

        skinColorButton.Select();
    }

    void OnDisable()
    {
        colorPickerUiController.onPickColor -= PickColor;
    }
}
