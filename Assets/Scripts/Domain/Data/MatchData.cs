using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    [SerializeField] private String hostPlayer;

    [SerializeField] private List<String> players;

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
