using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

    private bool isMatchStart = false;
    public bool IsMatchStart() => isMatchStart;
    public event Action onMatchStarting;

    [Header("Dependencies")]
    private PlayerData playerData;
    [SerializeField] private Transform playerInputObject;
    [SerializeField] private MatchReadyOverlayUiController matchReadyOverlayUiController;

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

        if (MatchData.players.Count == 1)
        {
            StartCoroutine(StartMatch());
            return;
        }

        // Only the host sends LOAD_MATCH to tell the client to load the scene
        if (IsPlayerHost())
        {
            StartCoroutine(SendLoadMatchToClient());
        }
        else
        {
            StartCoroutine(SendReadyToHost());
        }

        TabVisibilityService.instance.OnTabHidden += handleLostFocus;
    }

    public void ClientReady()
    {
        StartCoroutine(StartMatch());
    }

    IEnumerator StartMatch()
    {
        matchReadyOverlayUiController.SetText("3");
        yield return new WaitForSecondsRealtime(1f);

        matchReadyOverlayUiController.SetText("2");
        yield return new WaitForSecondsRealtime(1f);

        matchReadyOverlayUiController.SetText("1");
        yield return new WaitForSecondsRealtime(1f);

        matchReadyOverlayUiController.SetText("Fight!!!");

        isMatchStart = true;

        yield return new WaitForSecondsRealtime(0.7f);

        matchReadyOverlayUiController.CloseWindow();
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

    private IEnumerator SendReadyToHost()
    {
        // Wait a frame to ensure everything is initialized after scene load
        yield return null;

        Debug.Log("[MatchManager] Client sending READY packet to host");
        P2PManager.instance.SendJson(new Packet()
        {
            type = "READY"
        }); 

        ClientReady();
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
        return hostPlayer.CompareTo(PlayerData.instance.username) == 0;
    }
   
    public void SetPlayerHost(string player)
    {
        Debug.Log($"[MatchManager] Setting new host: {player} (local player: {PlayerData.instance.username})");

        hostPlayer = player;

        bool isLocalPlayerNewHost = player.CompareTo(PlayerData.instance.username) == 0;
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
        if (!IsPlayerHost() || !isMatchStart) return;

        string newHost = GetClientPlayer();

        Debug.Log("Lost Focus");

        try
        {
            if (HostPacketSender.instance != null) 
                HostPacketSender.instance.sendNewHost(newHost);
        }
        catch (Exception e)
        {
            return;
        }

        SetPlayerHost(newHost);
    }
}
