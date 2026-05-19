using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerInputHandler : IPlayerInputHandler
{
    public override void ControlCharacterAction(string player, CharacterActions action, bool state)
    {
        throw new System.NotImplementedException();
    }

    public override void ControlCharacterRotation(string player, Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
}
