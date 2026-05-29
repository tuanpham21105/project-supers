using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class Message
{
    public string type;
    public string sender;
    public string receiver;
    public string matchId;
    public JToken value; // ← JToken chứa được cả string lẫn object

    public T GetValue<T>() => value.ToObject<T>();
}
