using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerInputHandler : IPlayerInputHandler
{
    void Start()
    {
        BaseStart();

        ClientPacketReceiver.instance.onFlyingInterrupted += HandleFlyingInterrupted;
    }

    void OnDestroy()
    {
        ClientPacketReceiver.instance.onFlyingInterrupted -= HandleFlyingInterrupted;
    }

    public override void ControlCharacterAction(CharacterActions action, bool state)
    {
        ClientPacketSender.instance.sendControlAction(action, state);
    }

    public override void ControlCharacterRotation(Vector3 direction)
    {
        ClientPacketSender.instance.sendControlRotation(direction);
    }
}
