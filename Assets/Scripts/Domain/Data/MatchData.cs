using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchData
{
    public static string matchId;
    public static string hostPlayer;
    public static List<string> players = new List<string>();
}

public class MatchDto
{
    public string matchId;
    public string hostPlayer;
    public List<string> players;
}
