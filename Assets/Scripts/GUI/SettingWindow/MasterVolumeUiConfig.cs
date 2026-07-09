using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeUiConfig : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueTextField;
    [SerializeField] private Slider slider;

    public void OnSliderChange()
    {
        valueTextField.text = slider.value.ToString();
        PlayerData.instance.MasterVolume = (int) slider.value;
    }

    void OnEnable()
    {
        valueTextField.text = PlayerData.instance.MasterVolume.ToString();
        slider.value = PlayerData.instance.MasterVolume;
    }
}
