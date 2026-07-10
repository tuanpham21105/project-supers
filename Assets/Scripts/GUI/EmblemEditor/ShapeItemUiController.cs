using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeItemUiController : MonoBehaviour
{
    public event Action<int> onSelected;
    [SerializeField] private int index;
    private Sprite shapeSprite;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameTextField;

    public void Initialize(int index, Sprite shape, string name)
    {
        this.index = index;
        shapeSprite = shape;
        image.sprite = shape;
        nameTextField.text = name;
    }

    public void OnSelected()
    {
        onSelected?.Invoke(index);
    }
}
