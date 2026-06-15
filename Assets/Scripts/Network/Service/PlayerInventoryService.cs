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
}