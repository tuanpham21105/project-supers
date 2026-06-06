using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAccountService : MonoBehaviour
{
    public static PlayerAccountService instance;

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

    /// <summary>
    /// Retrieves the current player's account details.
    /// </summary>
    public void GetPlayerAccount(Action<PlayerAccountResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAccountResponse>(
            "GET",
            "/api/player/account/me",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    /// <summary>
    /// Retrieves another player's public account details by username.
    /// </summary>
    public void GetPlayerAccountByUsername(string username, Action<OtherPlayerAccountResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<OtherPlayerAccountResponse>(
            "GET",
            $"/api/player/account/{username}",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    /// <summary>
    /// Updates the current player's account details (email, username, password).
    /// </summary>
    public void UpdatePlayerAccount(PlayerAccountUpdateRequest request, Action<PlayerAccountResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAccountResponse>(
            "PUT",
            "/api/player/account",
            request,
            null,
            onSuccess,
            onError
        ));
    }

    /// <summary>
    /// Updates the current player's password.
    /// </summary>
    public void UpdatePlayerPassword(PlayerAccountUpdatePasswordRequest request, Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "PATCH",
            "/api/player/account/password",
            request,
            null,
            onSuccess,
            onError
        ));
    }

    /// <summary>
    /// Deletes the current player's account.
    /// </summary>
    public void DeletePlayerAccount(Action<MessageResponse<string>> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<MessageResponse<string>>(
            "DELETE",
            "/api/player/account",
            null,
            null,
            onSuccess,
            onError
        ));
    }
}
