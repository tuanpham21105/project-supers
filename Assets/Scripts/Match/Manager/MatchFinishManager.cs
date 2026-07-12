using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MatchFinishManager : MonoBehaviour
{
    public static MatchFinishManager instance;

    public event Action<int> onMatchFinish;

    public bool isFinish = false;

    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void Finish(String winner = "")
    {
        if (isFinish)
            return;

        Debug.LogWarning($"Match Finish {winner}");

        isFinish = true;

        if (winner.Equals(""))
        {
            onMatchFinish?.Invoke(0);
        }
        else if (PlayerData.instance.username.Equals(winner))
        {
            onMatchFinish?.Invoke(1);
        }
        else
        {
            onMatchFinish?.Invoke(-1);
        }

        StartCoroutine(EndMatch(winner));
    }

    IEnumerator EndMatch(string winner)
    {
        PlayerMatchService.instance.FinishMatch(
            MatchData.matchId, 
            winner,
            (response) =>
            {
                Debug.Log($"[MatchFinishManager] {response.message}");
            },
            (code, error) =>
            {
                Debug.LogError($"[MatchFinishManager] {error}");
            }
        );

        yield return new WaitForSecondsRealtime(5f);

        P2PManager.instance.DestroyPeer();

        SceneService.instance.LoadScene("MatchResultScene");
    }
}