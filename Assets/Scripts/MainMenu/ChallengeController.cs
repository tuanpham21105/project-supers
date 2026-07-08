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
        instance = null;
        WebSocketService.instance.OnMessageReceived -= handleWsMessage;
    }

    void handleWsMessage(WsMessage wsMessage)
    {
        if (wsMessage.type.Equals("CHALLENGE_RESPONSE"))
        {
            MatchResponse content = wsMessage.content.ToObject<MatchResponse>();

            MatchMakingController.instance.CancelMatchMaking();
            MatchMakingController.instance.handleMatchMakingSuccess(content);
            MatchMakingController.instance.OnMatchFoundWsMessageReceived(new WsMessage()
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
        if (!P2PManager.instance.IsInitialized)
        { 
            P2PManager.instance.OnReady += SendChallengeWaitForPeerReady(username);
            P2PManager.instance.Init(PlayerData.instance.username);
        }
        else
        {
            RealSendChallenge(username);
        }
    }

    Action SendChallengeWaitForPeerReady(string username)
    {
        return () =>
        {
            RealSendChallenge(username);
        };
    }

    void RealSendChallenge(string username)
    {
        P2PManager.instance.OnReady -= SendChallengeWaitForPeerReady(username);

        PlayerMatchService.instance.ChallengePlayer(
            new ChallengeRequest()
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
        if (!P2PManager.instance.IsInitialized)
        { 
            P2PManager.instance.OnReady += ResponseChallengeWaitForPeerReady(id, state);
            P2PManager.instance.Init(PlayerData.instance.username);
        }
        else
        {
            RealResponseChallenge(id, state);
        }
    }

    Action ResponseChallengeWaitForPeerReady(string id, bool state)
    {
        return () =>
        {
            RealResponseChallenge(id, state);
        };
    }

    void RealResponseChallenge(string id, bool state)
    {
        P2PManager.instance.OnReady -= ResponseChallengeWaitForPeerReady(id, state);

        if (state)
        {

            MatchMakingController.instance.CancelMatchMaking();
        }

        PlayerMatchService.instance.ResponseChallenge(
            new ChallengeResponseRequest()
            {
                id = id,
                response = state
            },
            (response) =>
            {
                if (state)
                    MatchMakingController.instance.handleMatchMakingSuccess(response);
            },
            (code, error) =>
            {
                Debug.LogError("Challenge no longer valid");
            }
        );  
    }
}
