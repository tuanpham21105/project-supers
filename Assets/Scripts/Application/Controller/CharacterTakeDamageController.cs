using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTakeDamageController : MonoBehaviour
{
    [SerializeField] private CharacterStatsData characterStatsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterObjectService characterObjectService;
    [SerializeField] private CharacterDebuffController characterDebuffController;

    void Start()
    {
        if (characterStatsData == null) characterStatsData = GetComponent<CharacterStatsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();    
        if (characterObjectService == null) characterObjectService = GetComponent<CharacterObjectService>();
        if (characterDebuffController == null) characterDebuffController = GetComponent<CharacterDebuffController>();

        characterStatesData.currentEndurance = characterStatsData.endurance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetHit(GameObject attacker, int damage, Vector3 direction, AttackTypes attackType)
    {
        if (attacker == gameObject) return;

        if (characterStatesData.knockAwayFlag)
        {
            
        }
        else
        {
            bool isFront = characterObjectService.IsPointFront(direction);

            if (isFront)
            {
                if (characterStatesData.blockFlag)
                {
                    damage = (int)((float)damage * characterStatsData.blockThreshold);
                    ApplyDamage(damage, isFront, direction);
                    return;
                }

                if (characterStatesData.deflectFlag)
                {
                    switch (attackType)
                    {
                        case AttackTypes.normal_attack:
                            attacker.GetComponent<CharacterDebuffController>().Deflected(direction);
                            return;
                    }
                }
            }

            ApplyDamage(damage, isFront, direction);
        }
    }

    private void ApplyDamage(int damage, bool isFront, Vector3 direction)
    {
        if (damage <= 0) return;

        if (damage >= (int)(characterStatsData.knockOutThreshold * (float)characterStatesData.currentEndurance))
        {
            characterDebuffController.KnockOut(direction, isFront, damage);
        }
        else
        {
            characterDebuffController.Hit(direction);
        }

        characterStatesData.currentEndurance -= damage;

        if (characterStatesData.currentEndurance < 0)
            characterStatesData.currentEndurance = 0;
    }
}
