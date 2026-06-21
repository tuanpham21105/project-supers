using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUiController : MonoBehaviour
{
    public Color selectedColor;

    public event Action<Color> onPickColor;

    public void Select(Color color)
    {
        selectedColor = color; 
        onPickColor?.Invoke(color);       
    }
}
