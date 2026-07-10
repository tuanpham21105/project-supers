using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecalPropertiesUiController : MonoBehaviour
{
    [SerializeField] private ColorPickerUiController colorPickerUiController;
    [SerializeField] private ColorLevelButtonUiController colorLevelButtonUiController;
    [SerializeField] private Slider xPosSlider;
    [SerializeField] private Slider yPosSlider;
    [SerializeField] private Slider rotateSlider;
    [SerializeField] private Slider scaleSlider;

    public event Action<Color> onColorSelected;
    public event Action<float> onXPosChange;
    public event Action<float> onYPosChange;
    public event Action<float> onScaleChange;
    public event Action<int> onRotateChange;
    
    private float posStep = 0.01f;
    private float scaleStep = 0.1f;
    private int rotateStep = 30;

    void OnEnable()
    {
        colorPickerUiController.onPickColor += handleColorSelected;
    }

    void OnDisable()
    {
        colorPickerUiController.onPickColor -= handleColorSelected;
    }

    void handleColorSelected(Color color)
    {
        colorLevelButtonUiController.SetColor(color);
        onColorSelected?.Invoke(color);
    }

    public void handleXPosChange()
    {
        onXPosChange?.Invoke(Mathf.CeilToInt(xPosSlider.value) * posStep);
    }

    public void handleYPosChange()
    {
        onYPosChange?.Invoke(Mathf.CeilToInt(yPosSlider.value) * posStep);
    }

    public void handleScaleChange()
    {
        onScaleChange?.Invoke(Mathf.CeilToInt(scaleSlider.value) * scaleStep);
    }

    public void handleRotateChange()
    {
        onRotateChange?.Invoke(Mathf.CeilToInt(rotateSlider.value) * rotateStep);
    }

    public void SetProperties(Color color, float xPos, float yPos, float scale, int rotate)
    {
        colorLevelButtonUiController.SetColor(color);
        xPosSlider.value = Mathf.CeilToInt(xPos / posStep);
        yPosSlider.value = Mathf.CeilToInt(yPos / posStep);
        scaleSlider.value = Mathf.CeilToInt(scale / scaleStep);
        rotateSlider.value = Mathf.CeilToInt(rotate / rotateStep);
    }
}
