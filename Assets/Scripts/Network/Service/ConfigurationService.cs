using System;
using UnityEngine;

public class ConfigurationService : MonoBehaviour
{
    public static ConfigurationService instance;

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

    public void GetKeyboardConfiguration(Action<KeyboardConfigurationResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<KeyboardConfigurationResponse>(
            "GET",
            "/api/player/configuration/keyboard",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void PutKeyboardConfiguration(KeyboardConfigurationRequest request, Action<KeyboardConfigurationResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<KeyboardConfigurationResponse>(
            "PUT",
            "/api/player/configuration/keyboard",
            request,
            null,
            onSuccess,
            onError
        ));
    }
}