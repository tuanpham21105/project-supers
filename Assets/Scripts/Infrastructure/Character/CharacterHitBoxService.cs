using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitBoxService : MonoBehaviour
{
    [SerializeField] private CharacterHitBoxesEvents characterHitBoxesEvents;

    [SerializeField] private AttackTypes attackType;

    private void OnTriggerEnter(Collider other)
    {
        if (characterHitBoxesEvents == null) return;

        switch (attackType)
        {
            case AttackTypes.light_attack:
                characterHitBoxesEvents.EmitNormalAttack(other.gameObject);
                break;
            case AttackTypes.heavy_attack:
                characterHitBoxesEvents.EmitStrikeAttack(other.gameObject);
                break;
        }
    }
}
