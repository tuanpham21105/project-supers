using System;
using UnityEngine;

public class StoreService : MonoBehaviour
{
    public static StoreService instance;

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