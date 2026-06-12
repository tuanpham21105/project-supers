using System;
using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using Newtonsoft.Json;
using UnityEngine;

public class ClientPacketReceiver : MonoBehaviour
{
    public static ClientPacketReceiver instance;
    
    private MatchManager matchManager;
    
    public event Action<string> onFlyingInterrupted;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[ClientPacketReceiver] P2PManager dependency is missing.");
            return;
        }

        matchManager = MatchManager.instance;

        P2PManager.instance.OnReliableData += handleReceiveMessage;
        P2PManager.instance.OnUnreliableData += handleReceiveMessage;
    }

    void OnDestroy()
    {

        P2PManager.instance.OnReliableData -= handleReceiveMessage;
        P2PManager.instance.OnUnreliableData -= handleReceiveMessage;
    }

    void handleReceiveMessage(string data)
    {
        Packet packet = JsonConvert.DeserializeObject<Packet>(data);

        if (packet.type.CompareTo("FLYING_INTERRUPTED") == 0)
        {
            FlyingInterruptedEventPacket packet1 = JsonConvert.DeserializeObject<FlyingInterruptedEventPacket>(data);
            receivePlayerCharacterFlyingInterrupted(packet1);
        }
        else if (packet.type.CompareTo("ANIMATION") == 0)
        {
            AnimationEventPacket packet2 = JsonConvert.DeserializeObject<AnimationEventPacket>(data);
            receivePlayerCharacterAnimation(packet2);
        }
        else if (packet.type.CompareTo("STATES") == 0)
        {
            StatesPacket packet3 = JsonConvert.DeserializeObject<StatesPacket>(data);
            receivePlayerCharacterStates(packet3);
        }
        else if (packet.type.CompareTo("LOST_FOCUS") == 0)
        {
            // MatchManager.instance.emitHostLostFocus();
        }
        else if (packet.type.CompareTo("GAIN_FOCUS") == 0)
        {
            // MatchManager.instance.emitHostGainFocus();
        }
        else if (packet.type.CompareTo("SURRENDER") == 0)
        {
            Debug.Log($"[HostPacketReceiver] {PlayerData.instance.username} surrender.");

            ClientPacketSender.instance.sendSurrenderAcknowledge();

            MatchFinishManager.instance.Finish(PlayerData.instance.username);
        }
    } 

    void receivePlayerCharacterFlyingInterrupted(FlyingInterruptedEventPacket packet)
    {
        onFlyingInterrupted?.Invoke(packet.player);
        Debug.Log($"[ClientPacketReceiver] {packet.player} flying is interrupted.");
    }

    void receivePlayerCharacterAnimation(AnimationEventPacket packet)
    {
        CharactersManager.instance.ControlCharacterAnimation(packet.player, packet.animationType, packet.animation);
    }

    void receivePlayerCharacterStates(StatesPacket packet)
    {
        foreach (string a in matchManager.GetPlayers())
        {
            CharacterStatesDto states = packet.data.playersStates[a];
            CharactersManager.instance.ControlCharacterStates(a, states);
        }
    }
}
