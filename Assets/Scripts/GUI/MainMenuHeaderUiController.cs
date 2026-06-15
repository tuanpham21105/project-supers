using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuHeaderUiController : MonoBehaviour
{
    public static MainMenuHeaderUiController instance;

    [SerializeField] private TextMeshProUGUI usernameTextField;
    [SerializeField] private TextMeshProUGUI pointsTextField;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // SetHeaderUsername();
        // SetPoints();
    }

    public void SetHeaderUsername()
    {
        usernameTextField.text = PlayerData.instance.username;   
    }

    public void SetPoints()
    {
        long points = PlayerData.instance.points;

        if (points < 1_000)
            pointsTextField.text = points.ToString();
        else if (points < 1_000_000)
            pointsTextField.text = (points / 1_000.0).ToString("0.0") + "K";
        else if (points < 1_000_000_000)
            pointsTextField.text = (points / 1_000_000.0).ToString("0.0") + "M";
        else if (points < 1_000_000_000_000)
            pointsTextField.text = (points / 1_000_000_000.0).ToString("0.0") + "B";
        else
            pointsTextField.text = (points / 1_000_000_000_000.0).ToString("0.0") + "T";
    }
}
