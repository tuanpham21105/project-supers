using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetworkData
{
    public static string BaseUrl() => "localhost:8080";
    public static string BaseRestSchema() => "http://";
    public static string BaseWebSocketSchema() => "ws://";
}
