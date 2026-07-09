using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MouseConfigUiController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sensitivityValueTextField;
    [SerializeField] private Slider sensitivitySlider;

    public void OnSliderChange()
    {
        SetSensitivityValueText(sensitivitySlider.value);
    }

    public void ApplySensitivityToUi(float value)
    {
        sensitivitySlider.value = value;
        SetSensitivityValueText(value);
    }

    public float GetSensitivityFromUi()
    {
        return sensitivitySlider.value;
    }

    private void SetSensitivityValueText(float value)
    {
        sensitivityValueTextField.text = value.ToString("F2");
    }
}
