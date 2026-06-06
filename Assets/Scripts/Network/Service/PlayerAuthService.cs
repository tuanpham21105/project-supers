using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuthService : MonoBehaviour
{
    public static PlayerAuthService instance;

    public const string AccessTokenKey = "accessToken";
    public const string RefreshTokenKey = "refreshToken";

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
    /// Creates a guest account.
    /// Saves the received access and refresh tokens into cookies.
    /// </summary>
    public void CreateGuestAccount(Action<PlayerAuthResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequest<PlayerAuthResponse>(
            "POST",
            "/api/player/auth/guest",
            null,
            null,
            (response) =>
            {
                SaveTokens(response);
                onSuccess?.Invoke(response);
            },
            onError
        ));
    }

    /// <summary>
    /// Logs in a player using email and password.
    /// Saves the received access and refresh tokens into cookies.
    /// </summary>
    public void Login(string email, string password, Action<PlayerAuthResponse> onSuccess, Action<long, string> onError)
    {
        PlayerLoginRequest body = new PlayerLoginRequest
        {
            email = email,
            password = password
        };

        StartCoroutine(RestApiService.instance.SendRequest<PlayerAuthResponse>(
            "POST",
            "/api/player/auth/login",
            body,
            null,
            (response) =>
            {
                SaveTokens(response);
                onSuccess?.Invoke(response);
            },
            onError
        ));
    }

    /// <summary>
    /// Creates a new player account with email, username, and password.
    /// Saves the received access and refresh tokens into cookies.
    /// </summary>
    public void SignUp(string email, string username, string password, Action<PlayerAuthResponse> onSuccess, Action<long, string> onError)
    {
        PlayerSignUpRequest body = new PlayerSignUpRequest
        {
            email = email,
            username = username,
            password = password
        };

        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAuthResponse>(
            "POST",
            "/api/player/auth/signup",
            body,
            null,
            (response) =>
            {
                SaveTokens(response);
                onSuccess?.Invoke(response);
            },
            onError
        ));
    }

    /// <summary>
    /// Refreshes the access token using the stored refresh token from cookies.
    /// </summary>
    public void RefreshAccessToken(Action<PlayerAuthResponse> onSuccess, Action<long, string> onError)
    {
        string refreshToken = CookieService.Get(RefreshTokenKey);
        if (string.IsNullOrEmpty(refreshToken))
        {
            onError?.Invoke(0, "No refresh token found in cookies.");
            return;
        }

        RefreshAccessToken(refreshToken, onSuccess, onError);
    }

    /// <summary>
    /// Refreshes the access token using a specified refresh token.
    /// Saves the newly received access and refresh tokens into cookies.
    /// </summary>
    public void RefreshAccessToken(string refreshToken, Action<PlayerAuthResponse> onSuccess, Action<long, string> onError)
    {
        PlayerRefreshRequest body = new PlayerRefreshRequest
        {
            refreshToken = refreshToken
        };

        StartCoroutine(RestApiService.instance.SendRequest<PlayerAuthResponse>(
            "POST",
            "/api/player/auth/refresh",
            body,
            null,
            (response) =>
            {
                SaveTokens(response);
                onSuccess?.Invoke(response);
            },
            onError
        ));
    }

    /// <summary>
    /// Clears any stored access and refresh tokens from cookies.
    /// </summary>
    public void Logout()
    {
        CookieService.Remove(AccessTokenKey);
        CookieService.Remove(RefreshTokenKey);
        Debug.Log("[PlayerAuthService] Cleared tokens from cookies.");
    }

    /// <summary>
    /// Helper to save auth response tokens to cookies.
    /// </summary>
    private void SaveTokens(PlayerAuthResponse response)
    {
        if (response != null)
        {
            if (!string.IsNullOrEmpty(response.accessToken))
            {
                CookieService.Set(AccessTokenKey, response.accessToken);
                Debug.Log("[PlayerAuthService] Saved access token to cookies.");
            }
            if (!string.IsNullOrEmpty(response.refreshToken))
            {
                CookieService.Set(RefreshTokenKey, response.refreshToken);
                Debug.Log("[PlayerAuthService] Saved refresh token to cookies.");
            }
        }
    }
}
