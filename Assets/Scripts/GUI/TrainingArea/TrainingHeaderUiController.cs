using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingHeaderUiController : MonoBehaviour
{
    public static TrainingHeaderUiController instance;

    [SerializeField] private GameObject leftSide;

    [SerializeField] private TextMeshProUGUI leftPlayerUsername;
    [SerializeField] private Image leftPlayerHealthBar;
    [SerializeField] private DashCooldownBarUiController leftPlayerDashCooldownBar;
    
    [SerializeField] private RawImage leftEmblem;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupUi();

        TrainingCharacterManager.instance.onCharacterDashStart += handleDashStart;
        TrainingCharacterManager.instance.onCharacterDashCooldownStart += handleDashStartCooldown;
        TrainingCharacterManager.instance.onCharacterDashCooldownEnd += handleDashCooldownEnd;
    }

    void OnDestroy()
    {
        if (TrainingCharacterManager.instance != null)
        {
            TrainingCharacterManager.instance.onCharacterDashStart -= handleDashStart;
            TrainingCharacterManager.instance.onCharacterDashCooldownStart -= handleDashStartCooldown;
            TrainingCharacterManager.instance.onCharacterDashCooldownEnd -= handleDashCooldownEnd;
        }

        instance = null;
    }

    void SetupUi()
    {
        leftPlayerUsername.text = MatchData.hostPlayer;
    }

    public void SetPlayerHealth(float healthPercent)
    {
        leftPlayerHealthBar.fillAmount = healthPercent;
    }

    public void SetPlayerEmblem(Material material)
    {

        Texture tex = material.GetTexture("_ShadowTex");
        leftEmblem.texture = tex;
    }

    void handleDashStart()
    {
        leftPlayerDashCooldownBar.StartDash();
    }

    void handleDashStartCooldown(float duration)
    {
        leftPlayerDashCooldownBar.StartDashCooldown(duration);
    }

    void handleDashCooldownEnd()
    {
        leftPlayerDashCooldownBar.EndDashCooldown();
    }
}
