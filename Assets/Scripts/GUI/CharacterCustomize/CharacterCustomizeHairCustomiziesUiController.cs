using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeHairCustomiziesUiController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private ColorPickerUiController colorPickerUiController;
    [SerializeField] private List<ColorLevelButtonUiController> colorLevelButtons = new List<ColorLevelButtonUiController>();
    [SerializeField] private int selectedColorLevel;

    //
    public event Action<Color, Color, Color> onPickColor;

    void OnEnable()
    {
        colorPickerUiController.onPickColor += handlePickColor;

        SelectColorLevels(selectedColorLevel);
    }

    void OnDisable()
    {
        colorPickerUiController.onPickColor -= handlePickColor;
    }

    void handlePickColor(Color color)
    {
        colorLevelButtons[selectedColorLevel].SetColor(color);

        onPickColor?.Invoke(colorLevelButtons[0].GetColor(), colorLevelButtons[1].GetColor(), colorLevelButtons[2].GetColor());
    }

    public void SelectColorLevels(int level)
    {
        colorLevelButtons[selectedColorLevel].Unselect();

        selectedColorLevel = level;
        
        colorLevelButtons[selectedColorLevel].Select();
    }

    public void SetColors(Color primary, Color secondary, Color tertiary)
    {
        colorLevelButtons[0].SetColor(primary);
        colorLevelButtons[1].SetColor(secondary);
        colorLevelButtons[2].SetColor(tertiary);
    }
}
