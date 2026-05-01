using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitBoxService : MonoBehaviour
{
    [SerializeField] private CharacterHitBoxesEvents characterHitBoxesEvents;

    [SerializeField] private AttackTypes attackType;

    void Start()
    {
        characterHitBoxesEvents.OnAttackInterrupt += HandleAttackInterrupt;
    }

    void OnDestroy()
    {
        characterHitBoxesEvents.OnAttackInterrupt -= HandleAttackInterrupt;
    }

    private void HandleAttackInterrupt()
    {
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (characterHitBoxesEvents == null) return;

        switch (attackType)
        {
            case AttackTypes.light_attack:
                characterHitBoxesEvents.EmitNormalAttack(other.GetComponent<CharacterHurtBoxService>().GetCharacter());
                break;
            case AttackTypes.heavy_attack:
                characterHitBoxesEvents.EmitStrikeAttack(other.GetComponent<CharacterHurtBoxService>().GetCharacter());
                break;
        }
    }
}
