using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorLevelButtonUiController : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    
    public void SetColor(Color color)
    {
        colorImage.color = color;
    }

    public Color GetColor()
    {
        return colorImage.color;
    }

    public void Select()
    {
        GetComponent<Button>().interactable = false;
    }

    public void Unselect()
    {
        GetComponent<Button>().interactable = true;
    }
}
