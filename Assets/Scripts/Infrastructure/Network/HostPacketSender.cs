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
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        FlyingInterruptedEventPacket packet = new FlyingInterruptedEventPacket();
        packet.player = player;
    
        Debug.Log($"[HostPacketSender] {player} flying is interrupted.");

        P2PManager.instance.SendJson(packet);
    }

    public void sendPlayerCharacterAnimation(String player, String type, String animation)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        AnimationEventPacket packet = new AnimationEventPacket();
        packet.player = player;
        packet.animationType = type;
        packet.animation = animation;

        P2PManager.instance.SendJson(packet);
    }

    public void sendPlayersCharacterStates(PlayersCharacterStatesDto data)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        StatesPacket packet = new StatesPacket();
        packet.data = data;

        P2PManager.instance.SendJsonUnreliable(packet);
    }
}
