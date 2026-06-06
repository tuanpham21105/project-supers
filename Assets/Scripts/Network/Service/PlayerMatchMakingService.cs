using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMatchMakingService : MonoBehaviour
{
    public static PlayerMatchMakingService instance;

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
}
