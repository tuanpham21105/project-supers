using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTakeDamageController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterStatsData characterStatsData;
    private CharacterStatesData characterStatesData;
    private CharacterObjectService characterObjectService;
    private CharacterDebuffController characterDebuffController;

    void Start()
    {
        if (characterStatsData == null) characterStatsData = GetComponent<CharacterStatsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();    
        if (characterObjectService == null) characterObjectService = GetComponent<CharacterObjectService>();
        if (characterDebuffController == null) characterDebuffController = GetComponent<CharacterDebuffController>();

        characterStatesData.currentEndurance = characterStatsData.endurance;
    }
    
    private void ApplyDamage(int damage, bool isFront, Vector3 direction)
    {
        if (damage <= 0) return;

        characterStatesData.currentEndurance -= damage;

        int oldCurrentEndurance = characterStatesData.currentEndurance;

        if (characterStatesData.currentEndurance <= 0)
        {
            characterStatesData.currentEndurance = 0;
        }

        if (damage >= (int)(characterStatsData.knockOutThreshold * (float)oldCurrentEndurance))
        {
            characterDebuffController.KnockOut(direction, isFront, damage);
        }
        else
        {
            characterDebuffController.Hit(direction, damage);
        }
    }

    // [Control method]
    public void GetHit(GameObject attacker, int damage, Vector3 direction, AttackTypes attackType)
    {
        if (attacker == gameObject) return;

        if (characterStatesData.deadFlag) return;

        if (characterStatesData.knockAwayFlag)
        {
            // characterDebuffController.Dead();
        }
        else
        {
        }
            bool isFront = characterObjectService.IsPointFront(direction);

            if (isFront)
            {
                if (characterStatesData.blockFlag)
                {
                    damage = (int)((float)damage * (1f - characterStatsData.blockThreshold));
                    ApplyDamage(damage, isFront, direction);
                    return;
                }

                if (characterStatesData.deflectFlag)
                {
                    switch (attackType)
                    {
                        case AttackTypes.normal_attack:
                            attacker.GetComponent<CharacterDebuffController>().Deflected();
                            return;
                    }
                }
            }

            ApplyDamage(damage, isFront, direction);
    }

}
