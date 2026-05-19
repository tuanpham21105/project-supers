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

    public override void ControlCharacterAction(string player, CharacterActions action, bool state)
    {
        hostCharactersManager.ControlCharacterAction(player, action, state);
    }

    public override void ControlCharacterRotation(string player, Vector3 direction)
    {
        hostCharactersManager.ControlCharacterRotation(player, direction);
    }
}
