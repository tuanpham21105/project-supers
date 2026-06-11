using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchHeaderUiController : MonoBehaviour
{
    [SerializeField] private GameObject leftSide;
    [SerializeField] private GameObject middlePart;
    [SerializeField] private GameObject rightSide;

    [SerializeField] private TextMeshProUGUI leftPlayerUsername;
    [SerializeField] private Image leftPlayerHealthBar;

    [SerializeField] private TextMeshProUGUI rightPlayerUsername;
    [SerializeField] private Image rightPlayerHealthBar;

    void Start()
    {
        SetupUi();

        CharactersManager.instance.onCharacterHealthChange += SetPlayerHealth;
    }

    void OnDestroy()
    {
        if (CharactersManager.instance != null)
            CharactersManager.instance.onCharacterHealthChange -= SetPlayerHealth;
    }

    void SetupUi()
    {
        foreach (String player in MatchManager.instance.GetPlayers())
        {
            if (PlayerData.instance.username.CompareTo(player) == 0)
            {
                leftPlayerUsername.text = player;
            }
            else
            {
                rightPlayerUsername.text = player;
            }
        }
    }

    void SetPlayerHealth(String player, float healthPercent)
    {
        if (PlayerData.instance.username.CompareTo(player) == 0)
        {
            leftPlayerHealthBar.fillAmount = healthPercent;
        }
        else
        {
            rightPlayerHealthBar.fillAmount = healthPercent;
        }
    }
}
