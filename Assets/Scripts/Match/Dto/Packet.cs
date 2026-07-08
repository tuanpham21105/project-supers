using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Packet
{
    public string type;
}

[Serializable]
public class FlyingInterruptedEventPacket : Packet
{
    public string player;

    public FlyingInterruptedEventPacket()
    {
        type = "FLYING_INTERRUPTED";
    }
}

[Serializable]
public class StatesPacket : Packet
{
    public PlayersCharacterStatesDto data;

    public StatesPacket()
    {
        type = "STATES";
    }
}

[Serializable]
public class AnimationEventPacket : Packet
{
    public string player;
    public string animationType;
    public string animation;

    public AnimationEventPacket()
    {
        type = "ANIMATION";
    }
}

[Serializable]
public class ActionEventPacket : Packet
{
    public string action;
    public bool state;

    public ActionEventPacket()
    {
        type = "ACTION";
    }
}

[Serializable]
public class RotateActionEventPacket : Packet
{
    public Vec3 direction;

    public RotateActionEventPacket()
    {
        type = "ROTATION";
    }
}

[Serializable]
public class NewHostEventPacket : Packet
{
    public string newHost;

    public NewHostEventPacket()
    {
        type = "NEW_HOST";
    }
}

[Serializable]
public class ClientInfoPacket : Packet
{
    public String clientUsername;

    public ClientInfoPacket()
    {
        type = "CLIENT_INFO";
    }
}

[Serializable]
public class HitEventPacket : Packet
{
    public string player;
    public int damage;
    public bool isDeflected = false;

    public HitEventPacket()
    {
        type = "HIT_EVENT";
    }
}