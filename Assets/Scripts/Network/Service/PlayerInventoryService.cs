using System;
using UnityEngine;

public class PlayerInventoryService : MonoBehaviour
{
    public static PlayerInventoryService instance;

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

    public void GetPlayerInventory(Action<PlayerInventoryResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerInventoryResponse>(
            "GET",
            "/api/player/inventory",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void GetPlayerAccessoriesSet(Action<PlayerAccessoriesSetResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAccessoriesSetResponse>(
            "GET",
            "/api/player/inventory/accessories",
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void GetPlayerAccessoriesSetByUsername(String username, Action<PlayerAccessoriesSetResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAccessoriesSetResponse>(
            "GET",
            "/api/player/inventory/accessories/" + username,
            null,
            null,
            onSuccess,
            onError
        ));
    }

    public void SavePlayerAccessoriesSet(PlayerAccessoriesSetRequest request, Action<PlayerAccessoriesSetResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<PlayerAccessoriesSetResponse>(
            "PUT",
            "/api/player/inventory/accessories",
            request,
            null,
            onSuccess,
            onError
        ));
    }
}