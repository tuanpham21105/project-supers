using System.Runtime.InteropServices;
using UnityEngine;

public static class CookieService
{
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")] private static extern void   JS_SetCookie(string name, string value, int days);
    [DllImport("__Internal")] private static extern string JS_GetCookie(string name);
    [DllImport("__Internal")] private static extern void   JS_RemoveCookie(string name);

#endif

    /// <summary>Set cookie, days = 0 thì là session cookie (mất khi đóng browser)</summary>
    public static void Set(string name, string value, int days = 7)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JS_SetCookie(name, value, days);
#else
        PlayerPrefs.SetString("cookie_" + name, value);
        Debug.Log($"[Cookie] Set (Editor): {name} = {value}");
#endif
    }

    /// <summary>Get cookie, trả về null nếu không tìm thấy</summary>
    public static string Get(string name)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return JS_GetCookie(name);
#else
        string key = "cookie_" + name;
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : null;
#endif
    }

    /// <summary>Remove cookie</summary>
    public static void Remove(string name)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JS_RemoveCookie(name);
#else
        PlayerPrefs.DeleteKey("cookie_" + name);
        Debug.Log($"[Cookie] Remove (Editor): {name}");
#endif
    }

    /// <summary>Check cookie có tồn tại không</summary>
    public static bool Has(string name) => Get(name) != null;
}