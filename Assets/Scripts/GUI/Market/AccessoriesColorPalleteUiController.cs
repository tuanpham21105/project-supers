using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesColorPalleteUiController : MonoBehaviour
{
    [SerializeField] private ColorPickerUiController colorPickerUiController;

    [SerializeField] private List<ColorLevelButtonUiController> colorLevelButtons;
    [SerializeField] private int selectedLevel = 0;

    public event Action<Color, Color, Color> onColorsChange;

    void Start()
    {
        colorPickerUiController.onPickColor += handlePickColor;
    }

    void handlePickColor(Color color)
    {
        colorLevelButtons[selectedLevel].SetColor(color);

        onColorsChange?.Invoke(
            GetColorByLevel(0), 
            GetColorByLevel(1),
            GetColorByLevel(2)
        );
    }

    public void SelectColorLevel(int level)
    {
        colorLevelButtons[selectedLevel].Unselect();
        
        selectedLevel = level;

        colorLevelButtons[selectedLevel].Select();
    }

    public Color GetColorByLevel(int level)
    {
        return  colorLevelButtons[level].GetColor();       
    }

    public void SetColors(Color primary, Color secondary, Color tertiary)
    {
        colorLevelButtons[0].SetColor(primary);
        colorLevelButtons[1].SetColor(secondary);
        colorLevelButtons[2].SetColor(tertiary);
    }
}
