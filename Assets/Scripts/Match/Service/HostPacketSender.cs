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

    void OnDestroy()
    {
        instance = null;
    }

    public void sendPlayerCharacterFlyingInterrupted(String player)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
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

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
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

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
            return;
        }

        StatesPacket packet = new StatesPacket();
        packet.data = data;

        P2PManager.instance.SendJsonUnreliable(packet);
    }

    public void sendLostFocus()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
            return;
        }

        Packet packet = new Packet();
        packet.type = "LOST_FOCUS";

        P2PManager.instance.SendJson(packet);
    }

    public void sendGainFocus()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
            return;
        }

        Packet packet = new Packet();
        packet.type = "GAIN_FOCUS";

        P2PManager.instance.SendJson(packet);
    }

    public void sendSurrender()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        Packet packet = new Packet();
        packet.type = "SURRENDER";

        P2PManager.instance.SendJson(packet);
    }

    public void sendHitEvent(string player, int damage, bool isDeflected)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketSender] P2PManager dependency is missing.");
            return;
        }

        if (!MatchConnectionManager.instance.IsConnected)
        {
            // Debug.LogWarning("[HostPacketSender] Cannot send: not connected.");
            return;
        }

        // Verify sender is the host
        if (!MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[HostPacketSender] Only the host can send host actions.");
            return;
        }

        HitEventPacket packet = new HitEventPacket()
        {
            player = player,
            damage = damage,
            isDeflected = isDeflected
        };

        P2PManager.instance.SendJson(packet);
    }
}
