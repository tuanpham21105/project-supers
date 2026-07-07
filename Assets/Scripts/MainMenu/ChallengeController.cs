using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ChallengeController : MonoBehaviour
{
    public static ChallengeController instance;

    public event Action<string, string> onChallengeCome;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        WebSocketService.instance.OnMessageReceived += handleWsMessage;
    }

    void OnDestroy()
    {
        WebSocketService.instance.OnMessageReceived -= handleWsMessage;
    }

    void handleWsMessage(WsMessage wsMessage)
    {
        if (wsMessage.type.Equals("CHALLENGE_RESPONSE"))
        {
            MatchResponse content = wsMessage.content.ToObject<MatchResponse>();

            MatchMakingController.instance.CancelMatchMaking();
            MatchMakingController.instance.handleMatchMakingSuccess(content);
            MatchMakingController.instance.OnWsMessageReceived(new WsMessage()
            {
                type = "MATCH_FOUND"
            });
        }
        else if (wsMessage.type.Equals("CHALLENGE"))
        {
            Challenge content = wsMessage.content.ToObject<Challenge>();

            onChallengeCome?.Invoke(content.id, content.requesterUsername);
        }
    }

    public void SendChallenge(string username)
    {
        PlayerMatchService.instance.ChallengePlayer(
            new global::ChallengeRequest()
            {
                receiverUsername = username
            },
            (response) =>
            {
                
            },
            (code, error) =>
            {
                Debug.LogError("Invalid Challenge");
            }
        );
    }

    public void ResponseChallenge(string id, bool state)
    {
        PlayerMatchService.instance.ResponseChallenge(
            new ChallengeResponseRequest()
            {
                id = id,
                response = state
            },
            (response) =>
            {
                if (state)
                    MatchMakingController.instance.CancelMatchMaking();

                MatchMakingController.instance.handleMatchMakingSuccess(response);
            },
            (code, error) =>
            {
                Debug.LogError("Challenge no longer valid");
            }
        );
    }
}
