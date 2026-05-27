using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostPacketSender : MonoBehaviour
{
    public static HostPacketSender instance;

    void Awake()
    {
        instance = this;
    }

    public void sendPlayerCharacterFlyingInterrupted(String player)
    {
        FlyingInterruptedEventPacket packet = new FlyingInterruptedEventPacket();
        packet.player = player;

        PeerConnectionManager.Instance.Send(packet);
    }

    public void sendPlayerCharacterAnimation(String player, String type, String animation)
    {
        AnimationEventPacket packet = new AnimationEventPacket();
        packet.player = player;
        packet.animationType = type;
        packet.animation = animation;

        PeerConnectionManager.Instance.Send(packet);
    }

    public void sendPlayersCharacterStates(PlayersCharacterStatesDto data)
    {
        StatesPacket packet = new StatesPacket();
        packet.states = data;

        PeerConnectionManager.Instance.Send(packet, false);
    }
}
