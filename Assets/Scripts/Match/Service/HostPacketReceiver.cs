using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class HostPacketReceiver : MonoBehaviour
{
    public static HostPacketReceiver instance;

    private CharactersManager hostCharactersManager;
    private P2PManager p2PManager;
    private List<Action<string>> clientInputHandlers = new List<Action<string>>();

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (P2PManager.instance == null)
        {
            Debug.LogError("[HostPacketReceiver] P2PManager dependency is missing.");
            return;
        }

        hostCharactersManager = CharactersManager.instance;
        p2PManager = P2PManager.instance;

        foreach (string a in MatchManager.instance.GetPlayers())
        {
            if (a == MatchManager.instance.GetHostPlayer()) continue;

            Action<string> action = handleClientPacket(a);
            p2PManager.OnReliableData += action;
            p2PManager.OnUnreliableData += action;
            clientInputHandlers.Add(action);
        }
    }

    void OnDestroy()
    {
        instance = null;
        foreach (Action<string> a in clientInputHandlers)
        {
            p2PManager.OnReliableData -= a;
            p2PManager.OnUnreliableData -= a;
        }       
    }

    Action<string> handleClientPacket(string player)
    {
        return (data) =>
        {
            Packet packet = JsonConvert.DeserializeObject<Packet>(data);

            if (packet.matchId != MatchData.matchId)
            {
                Debug.LogWarning($"[HostPacketReceiver] Ignored stale packet from matchId={packet.matchId}, current={MatchData.matchId}");
                return;
            }

            if (packet.type.CompareTo("ACTION") == 0)
            {
                ActionEventPacket packet1 = JsonConvert.DeserializeObject<ActionEventPacket>(data);
                hostCharactersManager.ControlCharacterAction(player, Enum.Parse<CharacterActions>(packet1.action), packet1.state);
            }
            else if (packet.type.CompareTo("ROTATION") == 0)
            {
                RotateActionEventPacket packet2 = JsonConvert.DeserializeObject<RotateActionEventPacket>(data);
                hostCharactersManager.ControlCharacterRotation(player, packet2.direction.ToVector3());
            }
            else if (packet.type.CompareTo("READY") == 0)
            {
                MatchManager.instance.ClientReady();
            }
            else if (packet.type.CompareTo("SURRENDER_ACKNOWLEDGE") == 0)
            {
                Debug.Log($"[HostPacketReceiver] {PlayerData.instance.username} surrender have been acknowledge.");

                MatchManager.instance.Surrender();
            }
        };
    }
}
