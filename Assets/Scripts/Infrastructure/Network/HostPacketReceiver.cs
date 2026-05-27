using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostPacketReceiver : MonoBehaviour
{
    public static HostPacketReceiver instance;

    private HostCharactersManager hostCharactersManager;
    private PeerConnectionManager peerConnectionManager;
    private List<Action<Packet>> clientInputHandlers;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        hostCharactersManager = HostCharactersManager.instance;
        peerConnectionManager = PeerConnectionManager.Instance;

        foreach (string a in MatchManager.instance.GetPlayers())
        {
            if (a == MatchManager.instance.GetHostPlayer()) continue;

            Action<Packet> action = handleClientInput(a);
            peerConnectionManager.OnMessageReceived += action;
            clientInputHandlers.Add(action);
        }
    }

    Action<Packet> handleClientInput(string player)
    {
        return (packet) =>
        {
            if (packet is ActionEventPacket packet1)
            {
                hostCharactersManager.ControlCharacterAction(player, Enum.Parse<CharacterActions>(packet1.action), packet1.state);
            }
            else if (packet is RotateActionEventPacket packet2)
            {
                hostCharactersManager.ControlCharacterRotation(player, packet2.direction);
            }
        };
    }
}
