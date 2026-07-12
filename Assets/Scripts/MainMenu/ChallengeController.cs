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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        WebSocketService.instance.OnMessageReceived += handleWsMessage;
    }

    void OnDestroy()
    {
        instance = null;
        WebSocketService.instance.OnMessageReceived -= handleWsMessage;

        // Dọn dẹp các handler đang chờ Peer Ready, tránh gọi vào object đã bị destroy
        if (_pendingChallengeHandler != null)
        {
            P2PManager.instance.OnReady -= _pendingChallengeHandler;
            _pendingChallengeHandler = null;
        }

        if (_pendingResponseChallengeHandler != null)
        {
            P2PManager.instance.OnReady -= _pendingResponseChallengeHandler;
            _pendingResponseChallengeHandler = null;
        }
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

    private Action _pendingChallengeHandler;

    public void SendChallenge(string username)
    {
        if (!P2PManager.instance.IsInitialized)
        {
            // Hủy đăng ký challenge đang chờ trước đó (nếu có) — tránh gửi trùng
            if (_pendingChallengeHandler != null)
                P2PManager.instance.OnReady -= _pendingChallengeHandler;

            _pendingChallengeHandler = () =>
            {
                P2PManager.instance.OnReady -= _pendingChallengeHandler; // tự unsubscribe sau khi chạy
                _pendingChallengeHandler = null;
                RealSendChallenge(username);
            };

            P2PManager.instance.OnReady += _pendingChallengeHandler;
            P2PManager.instance.Init(PlayerData.instance.username);
        }
        else
        {
            RealSendChallenge(username);
        }
    }

    void RealSendChallenge(string username)
    {
        // Không cần -= ở đây nữa — việc unsubscribe đã được xử lý
        // ngay bên trong _pendingChallengeHandler ở SendChallenge()

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

    private Action _pendingResponseChallengeHandler;

    public void ResponseChallenge(string id, bool state)
    {
        if (!P2PManager.instance.IsInitialized)
        {
            // Hủy đăng ký response đang chờ trước đó (nếu có) — tránh xử lý trùng
            if (_pendingResponseChallengeHandler != null)
                P2PManager.instance.OnReady -= _pendingResponseChallengeHandler;

            _pendingResponseChallengeHandler = () =>
            {
                P2PManager.instance.OnReady -= _pendingResponseChallengeHandler; // tự unsubscribe sau khi chạy
                _pendingResponseChallengeHandler = null;
                RealResponseChallenge(id, state);
            };

            P2PManager.instance.OnReady += _pendingResponseChallengeHandler;
            P2PManager.instance.Init(PlayerData.instance.username);
        }
        else
        {
            RealResponseChallenge(id, state);
        }
    }

    void RealResponseChallenge(string id, bool state)
    {
        // Không cần -= ở đây nữa — việc unsubscribe đã được xử lý
        // ngay bên trong _pendingResponseChallengeHandler ở ResponseChallenge()

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
