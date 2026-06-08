using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuHeaderUiController : MonoBehaviour
{
    public static MainMenuHeaderUiController instance;

    [SerializeField] private TextMeshProUGUI usernameTextField;

    void Awake()
    {
        instance = this;
    }

    public void SetupHeaderUi()
    {
        usernameTextField.text = PlayerData.instance.username;   
    }
}
