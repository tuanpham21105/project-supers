using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Vec3
{
    public float x, y, z;

    public static Vec3 From(Vector3 v) => new Vec3 { x = v.x, y = v.y, z = v.z };
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public struct Quat
{
    public float x, y, z, w;

    public static Quat From(Quaternion q) => new Quat { x = q.x, y = q.y, z = q.z, w = q.w };
    public Quaternion ToQuaternion() => new Quaternion(x, y, z, w);
}

public class CharacterStatesDto
{
    public Vec3  position;
    public Vec3  forward;
    public float physicsColliderRadius;
    public float physicsColliderHeight;
}
