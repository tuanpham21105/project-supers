using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmblemLayerItemUiController : MonoBehaviour
{
    public event Action onSelected;
    public event Action onDeleted;

    [SerializeField] private Image bgrImage;
    [SerializeField] private Image image;

    [SerializeField] private Sprite selectedBgrSprite;
    [SerializeField] private Sprite normalBgrSprite;

    public void OnSelected()
    {
        onSelected?.Invoke();
        SetSelected(true);
    }

    public void OnDeleted()
    {
        onDeleted?.Invoke();
    }

    public void SetSelected(bool state)
    {
        bgrImage.sprite = state ? selectedBgrSprite : normalBgrSprite;
    }

    public void SetShape(Sprite shape)
    {
        image.sprite = shape;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }
}
