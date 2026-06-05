using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TabVisibilityService : MonoBehaviour
{
    public static TabVisibilityService instance;

    public event Action OnTabHidden;
    public event Action OnTabVisible;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RegisterVisibilityChange(string gameObjectName, string methodName);
#endif

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterVisibilityChange(gameObject.name, "OnVisibilityChange");
#endif
    }

    // Được gọi từ jslib qua SendMessage
    private void OnVisibilityChange(string state)
    {
        if (state == "hidden")
        {
            Debug.Log("[Visibility] Tab hidden");
            OnTabHidden?.Invoke();
        }
        else
        {
            Debug.Log("[Visibility] Tab visible");
            OnTabVisible?.Invoke();
        }
    }
}