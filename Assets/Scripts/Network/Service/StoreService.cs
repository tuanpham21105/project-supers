using System;
using UnityEngine;

public class StoreService : MonoBehaviour
{
    public static StoreService instance;

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

    public void GetStoreItemsByType(StoreItemsType type, Action<StoreItemsListResponse> onSuccess, Action<long, string> onError)
    {
        StartCoroutine(RestApiService.instance.SendRequestWithJwt<StoreItemsListResponse>(
            "GET",
            "/api/store/items/" + type.ToString(),
            null,
            null,
            onSuccess,
            onError
        ));
    }
}