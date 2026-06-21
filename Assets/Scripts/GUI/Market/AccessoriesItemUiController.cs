using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccessoriesItemUiController : MonoBehaviour
{
    [SerializeField] private string code;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI valueTextField;

    [SerializeField] private Image selectedBgrImage;

    public event Action<string> onPress;

    public void SetItem(string code, Image image, int value)
    {
        this.code = code;
        this.image = image;
        valueTextField.text = value.ToString();
    }

    public void OnPress()
    {
        onPress?.Invoke(code);
    }
}
