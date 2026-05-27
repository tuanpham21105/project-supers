using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packet
{
    public String type;

    public String getType() => type;
}

public class FlyingInterruptedEventPacket : Packet
{
    public String player;

    public FlyingInterruptedEventPacket()
    {
        type = "FLYING_INTERRUPTED";
    }
}

public class StatesPacket : Packet
{
    public PlayersCharacterStatesDto states;

    public StatesPacket()
    {
        type = "STATES";
    }
}

public class AnimationEventPacket : Packet
{
    public String player;
    public String animationType;
    public String animation;

    public AnimationEventPacket()
    {
        type = "ANIMATION";
    }
}

public class ActionEventPacket : Packet
{
    public String action;
    public bool state;

    public ActionEventPacket()
    {
        type = "ACTION";
    }
}

public class RotateActionEventPacket : Packet
{
    public Vector3 direction;

    public RotateActionEventPacket()
    {
        type = "ROTATION";
    }
}
