using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorSlotUiController : MonoBehaviour
{
    public void Press()
    {
        transform.parent.GetComponent<ColorPickerUiController>().Select(GetComponent<Image>().color);
    }
}
