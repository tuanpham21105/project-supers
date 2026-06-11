using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMatchService : MonoBehaviour
{
    public static PlayerMatchService instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartMatchMaking(Action<MatchResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MatchResponse>(
            "POST",
            "/api/player/match/find",
            null,
            null,
            onSuccess,
            onError
        ));
    }
    
    public void CancelMatchMaking(Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "DELETE",
            "/api/player/match/cancel",
            null,
            null,
            onSuccess,
            onError
        ));
    }
    
    public void GetMatchResultById(String id, Action<MatchResultResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MatchResultResponse>(
            "GET",
            "/api/player/match/" + id,
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void FinishMatch(String matchId, String winnerUsername, Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        FinishMatchRequest body = new FinishMatchRequest();
        body.id = matchId;
        body.winner = winnerUsername;
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "POST",
            "/api/player/match/finish",
            body,
            null,
            onSuccess,
            onError
        ));
    }
}
