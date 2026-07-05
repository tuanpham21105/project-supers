using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeEyesCustomiziesUiController : MonoBehaviour
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

        SelectColorLevels(0);
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

    public void SetColors(Color iris, Color sclera, Color eyelid)
    {
        colorLevelButtons[0].SetColor(iris);
        colorLevelButtons[1].SetColor(sclera);
        colorLevelButtons[2].SetColor(eyelid);
    }
}
