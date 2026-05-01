using System.Collections;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PlayerTestInputController : MonoBehaviour
{
    [SerializeField] private GameObject character;
    private CharacterAttackController attackController;
    private CharacterDefenseController defenseController;

    [Header("Defense Inputs")]
    public bool blockInput;
    public bool deflectInput;

    [Header("Attack Inputs")]
    public bool normalAttackInput;
    public bool strikeAttackInput;

    [ProButton]
    public void InputBlock() => blockInput = !blockInput;
    [ProButton]
    public void InputDeflect() => deflectInput = !deflectInput;
    [ProButton]
    public void InputNormalAttack() => normalAttackInput = !normalAttackInput;
    [ProButton]
    public void InputStrikeAttack() => strikeAttackInput = !strikeAttackInput;

    void Start()
    {
        if (character != null)
        {
            attackController = character.GetComponent<CharacterAttackController>();
            defenseController = character.GetComponent<CharacterDefenseController>();
        }
    }

    void Update()
    {
        if (defenseController != null)
        {
            defenseController.Block(blockInput);

            if (deflectInput)
            {
                defenseController.Deflect();
                // deflectInput = false;
            }
        }

        if (attackController != null)
        {
            if (normalAttackInput)
            {
                attackController.StartNormalAttack();
                // normalAttackInput = false;
            }

            if (strikeAttackInput)
            {
                attackController.StartStrikeAttack();
                // strikeAttackInput = false;
            }
        }
    }
}
