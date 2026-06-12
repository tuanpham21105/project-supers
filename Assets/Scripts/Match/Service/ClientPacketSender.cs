using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPacketSender : MonoBehaviour
{
    public static ClientPacketSender instance;

    private P2PManager p2PManager;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        p2PManager = P2PManager.instance;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void sendControlAction(CharacterActions action, bool state)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[ClientPacketSender] P2PManager dependency is missing.");
            return;
        }

        // Verify sender is the host
        // if (MatchManager.instance.IsPlayerHost())
        // {
        //     return;
        // }

        ActionEventPacket packet = new ActionEventPacket();
        packet.action = action.ToString();
        packet.state = state;

        P2PManager.instance.SendJson(packet);
    }

    public void sendControlRotation(Vector3 direction)
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[ClientPacketSender] P2PManager dependency is missing.");
            return;
        }

        // Verify sender is the host
        if (MatchManager.instance.IsPlayerHost())
        {
            // Debug.LogWarning("[ClientPacketSender] Host cannot send client actions.");
            return;
        }

        RotateActionEventPacket packet = new RotateActionEventPacket();
        packet.direction = Vec3.From(direction);

        P2PManager.instance.SendJsonUnreliable(packet);
    }

    public void sendSurrenderAcknowledge()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[ClientPacketSender] P2PManager dependency is missing.");
            return;
        }

        Packet packet = new Packet();
        packet.type = "SURRENDER_ACKNOWLEDGE";

        P2PManager.instance.SendJson(packet);
    }
}
