using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPacketReceiver : MonoBehaviour
{
    public static ClientPacketReceiver instance;
    
    private ClientCharactersManager clientCharactersManager;
    private ClientPlayerInputHandler clientPlayerInputHandler;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        clientCharactersManager = ClientCharactersManager.instance;

        PeerConnectionManager.Instance.OnMessageReceived += handleReceiveMessage;
    }

    void handleReceiveMessage(Packet packet)
    {
        if (packet is FlyingInterruptedEventPacket packet1)
        {
            receivePlayerCharacterFlyingInterrupted(packet1);
        }
        else if (packet is AnimationEventPacket packet2)
        {
            receivePlayerCharacterAnimation(packet2);
        }
        else if (packet is StatesPacket packet3)
        {
            receivePlayerCharacterStates(packet3);
        }
    } 

    void receivePlayerCharacterFlyingInterrupted(FlyingInterruptedEventPacket packet)
    {
        clientPlayerInputHandler.HandleFlyingInterrupted(packet.player);
    }

    void receivePlayerCharacterAnimation(AnimationEventPacket packet)
    {
        clientCharactersManager.ControlCharacterAnimation(packet.player, packet.animationType, packet.animation);
    }

    void receivePlayerCharacterStates(StatesPacket packet)
    {
        foreach (KeyValuePair<string, CharacterStatesDTO> a in packet.states.playersStates)
        {
            clientCharactersManager.ControlCharacterStates(a.Key, a.Value);
        }
    }
}
