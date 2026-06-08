using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorPopupUiController : WindowUiController
{
    [SerializeField] private TextMeshProUGUI errorTextField;

    public void SetError(String error)
    {
        errorTextField.text = error;
    }
}
