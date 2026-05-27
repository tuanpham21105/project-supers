using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostPlayerInputHandler : IPlayerInputHandler
{
    private HostCharactersManager hostCharactersManager;

    void Start()
    {
        hostCharactersManager = HostCharactersManager.instance;

        hostCharactersManager.onCharacterFlyingInterrupted += HandleFlyingInterrupted;
    }

    void OnDestroy()
    {
        hostCharactersManager.onCharacterFlyingInterrupted -= HandleFlyingInterrupted;
    }

    public override void ControlCharacterAction(CharacterActions action, bool state)
    {
        hostCharactersManager.ControlCharacterAction(PlayerData.instance.player, action, state);
    }

    public override void ControlCharacterRotation(Vector3 direction)
    {
        hostCharactersManager.ControlCharacterRotation(PlayerData.instance.player, direction);
    }
}
