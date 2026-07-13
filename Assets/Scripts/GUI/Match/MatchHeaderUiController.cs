using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchHeaderUiController : MonoBehaviour
{
    public static MatchHeaderUiController instance;

    [SerializeField] private GameObject leftSide;
    [SerializeField] private GameObject middlePart;
    [SerializeField] private GameObject rightSide;

    [SerializeField] private TextMeshProUGUI leftPlayerUsername;
    [SerializeField] private Image leftPlayerHealthBar;

    [SerializeField] private TextMeshProUGUI rightPlayerUsername;
    [SerializeField] private Image rightPlayerHealthBar;
    
    [SerializeField] private RawImage leftEmblem;
    [SerializeField] private RawImage rightEmblem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupUi();
    }

    void OnDestroy()
    {
        instance = null;
    }

    void SetupUi()
    {
        foreach (String player in MatchData.players)
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

    public void SetPlayerHealth(String player, float healthPercent)
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

    public void SetPlayerEmblem(String player, Material material)
    {

        Texture tex = material.GetTexture("_ShadowTex");
        if (PlayerData.instance.username.CompareTo(player) == 0)
        {
            leftEmblem.texture = tex;
        }
        else
        {
            rightEmblem.texture = tex;
        }
    }
}
