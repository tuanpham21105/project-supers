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
    [SerializeField] private DashCooldownBarUiController leftPlayerDashCooldownBar;

    [SerializeField] private TextMeshProUGUI rightPlayerUsername;
    [SerializeField] private Image rightPlayerHealthBar;
    [SerializeField] private DashCooldownBarUiController rightPlayerDashCooldownBar;
    
    [SerializeField] private RawImage leftEmblem;
    [SerializeField] private RawImage rightEmblem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupUi();

        CharactersManager.instance.onCharacterDashStart += handleDashStart;
        CharactersManager.instance.onCharacterDashCooldownStart += handleDashStartCooldown;
        CharactersManager.instance.onCharacterDashCooldownEnd += handleDashCooldownEnd;
    }

    void OnDestroy()
    {
        if (CharactersManager.instance != null)
        {
            CharactersManager.instance.onCharacterDashStart -= handleDashStart;
            CharactersManager.instance.onCharacterDashCooldownStart -= handleDashStartCooldown;
            CharactersManager.instance.onCharacterDashCooldownEnd -= handleDashCooldownEnd;
        }

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

    void handleDashStart(String player)
    {
        GetDashCooldownBar(player).StartDash();
    }

    void handleDashStartCooldown(String player, float duration)
    {
        GetDashCooldownBar(player).StartDashCooldown(duration);
    }

    void handleDashCooldownEnd(String player)
    {
        GetDashCooldownBar(player).EndDashCooldown();
    }

    DashCooldownBarUiController GetDashCooldownBar(String player)
    {
        if (PlayerData.instance.username.CompareTo(player) == 0)
            return leftPlayerDashCooldownBar;
        return rightPlayerDashCooldownBar;
    }
}
