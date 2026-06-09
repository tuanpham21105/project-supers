using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MatchReadyOverlayUiController : WindowUiController
{
    [SerializeField] TextMeshProUGUI readyTextField;

    public void SetText(string text)
    {
        readyTextField.text = text;
    }
}
