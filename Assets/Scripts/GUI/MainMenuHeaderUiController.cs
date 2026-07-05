using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuHeaderUiController : MonoBehaviour
{
    public static MainMenuHeaderUiController instance;

    [SerializeField] private TextMeshProUGUI usernameTextField;
    [SerializeField] private TextMeshProUGUI pointsTextField;
    [SerializeField] private TextMeshProUGUI levelsTextField;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // SetHeaderUsername();
        // SetPoints();

        PlayerData.instance.onPointsChange += SetPoints;
    }

    public void SetHeaderUsername()
    {
        usernameTextField.text = PlayerData.instance.username;   
    }

    public void SetPoints()
    {
        long points = PlayerData.instance.Points;
        pointsTextField.text = BigNumberStringify.decorate(points);
    }

    public void SetLevels()
    {
        levelsTextField.text = "Lvls." + PlayerData.instance.levels;
    }
}
