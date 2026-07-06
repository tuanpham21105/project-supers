using System;
using UnityEngine;

public class FriendService : MonoBehaviour
{
    public static FriendService instance;

    private string basePath = "/api/player/friend";

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

    public void GetAllFriends(Action<OtherPlayersListResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<OtherPlayersListResponse>(
            "GET",
            basePath + "/all",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void DeleteFriend(string username, Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "DELETE",
            basePath + "/" + username,
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void GetRecentPlayers(Action<OtherPlayersListResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<OtherPlayersListResponse>(
            "GET",
            basePath + "/recent",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void SendFriendRequest(SendFriendRequestRequest request, Action<FriendRequestResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<FriendRequestResponse>(
            "POST",
            basePath + "/send",
            request,
            null,
            onSuccess,
            onError
        ));
    }

    public void GetFriendRequests(Action<FriendRequestsListResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<FriendRequestsListResponse>(
            "GET",
            basePath + "/requests",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void RemoveFriendRequest(string id, Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "DELETE",
            basePath + "/requests/" + id,
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void RespondToFriendRequest(string id, bool state, Action<FriendRequestResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<FriendRequestResponse>(
            "PATCH",
            basePath + "/requests/" + id + "/response/" + (state ? "true" : "false"),
            null,
            null,
            onSuccess,
            onError
        ));
    }
}