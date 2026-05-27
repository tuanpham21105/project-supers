using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Prefab")]
    [SerializeField] private GameObject hostCharactersManagerPrefab;
    [SerializeField] private GameObject clientCharactersManagerPrefab;
    [SerializeField] private GameObject hostInputHandlerPrefab;
    [SerializeField] private GameObject clientInputHandlerPrefab;
    [SerializeField] private GameObject hostPacketManager;
    [SerializeField] private GameObject clientPacketManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerData = PlayerData.instance;

        // Assign data from MatchData
        hostPlayer = MatchData.hostPlayer;
        players = MatchData.players;

        if (IsPlayerHost(playerData.player))
        {
            Instantiate(hostCharactersManagerPrefab);

            Instantiate(hostInputHandlerPrefab);

            Instantiate(hostPacketManager);
        }
        else
        {
            Instantiate(clientCharactersManagerPrefab);

            Instantiate(clientInputHandlerPrefab);

            Instantiate(clientPacketManager);
        }
    }

    public String GetHostPlayer() => hostPlayer;

    public List<String> GetPlayers() => players;

    public int GetPlayerIndex(String player)
    {
        return players.FindIndex(p => p.CompareTo(player) == 0);
    }

    public bool IsPlayerHost(String player)
    {
        return hostPlayer.CompareTo(player) == 0;
    }
}
