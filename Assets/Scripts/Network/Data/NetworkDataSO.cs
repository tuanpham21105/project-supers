using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkData", menuName = "Network/Network Data")]
public class NetworkDataSO : ScriptableObject
{
    [SerializeField] private string baseUrl = "localhost:8080";
    [SerializeField] private string baseRestSchema = "http://";
    [SerializeField] private string baseWebSocketSchema = "ws://";

    public string BaseUrl() => baseUrl;
    public string BaseRestSchema() => baseRestSchema;
    public string BaseWebSocketSchema() => baseWebSocketSchema;
}
