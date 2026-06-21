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

    void Start()
    {
        colorPickerUiController.onPickColor += handlePickColor;
    }

    void handlePickColor(Color color)
    {
        colorLevelButtons[selectedLevel].SetColor(color);
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
}
