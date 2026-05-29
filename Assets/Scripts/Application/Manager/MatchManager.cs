using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    [Header("Dependencies")]
    private PlayerData playerData;
    [SerializeField] private Transform playerInputObject;

    [Header("Data")]
    [SerializeField] private String hostPlayer;

    [SerializeField] private List<String> players;

    void Awake()
    {
        instance = this;
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
    }

    void Start()
    {
        playerData = PlayerData.instance;

        // Assign data from MatchData
        if (MatchData.hostPlayer != null && MatchData.hostPlayer != "")
        {
            hostPlayer = MatchData.hostPlayer;
        }

        if (MatchData.players.Count > 0)
        {
            players = MatchData.players;
        }

        TabVisibilityService.instance.OnTabHidden += handleLostFocus;

        // Only the host sends LOAD_MATCH to tell the client to load the scene
        if (IsPlayerHost())
        {
            StartCoroutine(SendLoadMatchToClient());
        }
    }

    private IEnumerator SendLoadMatchToClient()
    {
        // Wait a frame to ensure everything is initialized after scene load
        yield return null;

        Debug.Log("[MatchManager] Host sending LOAD_MATCH packet to client");
        P2PManager.instance.SendJson(new Packet()
        {
            type = "LOAD_MATCH"
        });
    }

    void OnDestroy()
    {
        TabVisibilityService.instance.OnTabHidden -= handleLostFocus;
    }

    public String GetHostPlayer() => hostPlayer;

    public List<String> GetPlayers() => players;

    public int GetPlayerIndex(String player)
    {
        return players.FindIndex(p => p.CompareTo(player) == 0);
    }

    public bool IsPlayerHost()
    {
        return hostPlayer.CompareTo(PlayerData.instance.player) == 0;
    }
    [ProButton]
    public void SetPlayerHost(string player)
    {
        Debug.Log($"[MatchManager] Setting new host: {player} (local player: {PlayerData.instance.player})");

        hostPlayer = player;

        bool isLocalPlayerNewHost = player.CompareTo(PlayerData.instance.player) == 0;
        CharactersManager.instance.switchCharacterMode(isLocalPlayerNewHost);
    }

    public string GetClientPlayer()
    {   
        foreach (string a in players)
        {
            if (hostPlayer.CompareTo(a) != 0)
            {
                return a;
            }
        }

        return hostPlayer;
    }

    public void handleLostFocus()
    {
        if (!IsPlayerHost()) return;

        string newHost = GetClientPlayer();

        Debug.Log("Lost Focus");

        try
        {
            HostPacketSender.instance.sendNewHost(newHost);
        }
        catch (Exception e)
        {
            return;
        }

        SetPlayerHost(newHost);
    }
}
