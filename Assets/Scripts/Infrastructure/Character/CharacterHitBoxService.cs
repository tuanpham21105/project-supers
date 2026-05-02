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
        if (attackType == AttackTypes.fly_attack)
        {
            characterHitBoxesEvents.OnStartFlyAttack += HandleStartFlyAttack;
            characterHitBoxesEvents.OnEndFlyAttack += HandleEndFlyAttack;
        }
    }

    void OnDestroy()
    {
        characterHitBoxesEvents.OnAttackInterrupt -= HandleAttackInterrupt;
    }

    private void HandleAttackInterrupt()
    {
        GetComponent<Collider>().enabled = false;
    }

    private void HandleStartFlyAttack()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void HandleEndFlyAttack()
    {
        GetComponent<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (characterHitBoxesEvents == null) return;

        characterHitBoxesEvents.EmitAttackHit(other.GetComponent<CharacterHurtBoxService>().GetCharacter(), attackType);
    }
}
