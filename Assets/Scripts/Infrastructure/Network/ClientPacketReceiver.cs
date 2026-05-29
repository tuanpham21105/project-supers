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
    private ClientCharactersManager clientCharactersManager;
    
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

        clientCharactersManager = ClientCharactersManager.instance;
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
    } 

    void receivePlayerCharacterFlyingInterrupted(FlyingInterruptedEventPacket packet)
    {
        onFlyingInterrupted?.Invoke(packet.player);
        Debug.Log($"[ClientPacketReceiver] {packet.player} flying is interrupted.");
    }

    void receivePlayerCharacterAnimation(AnimationEventPacket packet)
    {
        clientCharactersManager.ControlCharacterAnimation(packet.player, packet.animationType, packet.animation);
    }

    void receivePlayerCharacterStates(StatesPacket packet)
    {
        foreach (string a in matchManager.GetPlayers())
        {
            CharacterStatesDTO states = packet.data.playersStates[a];
            clientCharactersManager.ControlCharacterStates(a, states);
        }
    }
}
