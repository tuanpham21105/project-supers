using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPacketSender : MonoBehaviour
{
    public static ClientPacketSender instance;

    private PeerConnectionManager peerConnectionManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        peerConnectionManager = PeerConnectionManager.Instance;
    }

    public void sendControlAction(CharacterActions action, bool state)
    {
        ActionEventPacket packet = new ActionEventPacket();
        packet.action = action.ToString();
        packet.state = state;

        peerConnectionManager.Send(packet);
    }

    public void sendControlRotation(Vector3 direction)
    {
        RotateActionEventPacket packet = new RotateActionEventPacket();
        packet.direction = direction;

        peerConnectionManager.Send(packet);
    }
}
