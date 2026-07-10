using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// ─────────────────────────────────────────────
// Custom Converter — lưu Color dưới dạng chuỗi hex trong JSON
// ─────────────────────────────────────────────
// public class ColorHexConverter : JsonConverter<Color>
// {
//     public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
//     {
//         writer.WriteValue("#" + ColorUtility.ToHtmlStringRGBA(value));
//     }

//     public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
//     {
//         string hex = (string)reader.Value;
//         ColorUtility.TryParseHtmlString(hex, out Color color);
//         return color;
//     }
// }

[Serializable]
public class Decal
{
    public int shapeIndex = 0;
    public Color color = Color.white;
    public float x = 0.5f;
    public float y = 0.5f;
    public float scale = 0.5f;
    public int rotate = 0;

    public Decal Clone()
    {
        return new Decal
        {
            shapeIndex = shapeIndex,
            color = color,
            x = x,
            y = y,
            scale = scale,
            rotate = rotate
        };
    }
}

[Serializable]
public class Emblem
{
    public List<Decal> decals = new List<Decal>();

    private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
    {
        Converters = { new ColorHexConverter() }
    };

    public Emblem Clone()
    {
        Emblem clone = new Emblem();
        clone.decals = new List<Decal>();

        for (int i = 0; i < decals.Count; i++)
        {
            clone.decals.Add(decals[i].Clone());
        }

        return clone;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, _settings);
    }

    public static Emblem FromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return new Emblem();

        return JsonConvert.DeserializeObject<Emblem>(json, _settings);
    }
}