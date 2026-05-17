using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class PlayerTestInputController : MonoBehaviour
{
    [SerializeField] private GameObject character;
    private CharacterAttackController attackController;
    private CharacterDefenseController defenseController;
    private CharacterMovementController movementController;
    private CharacterStatesData characterStatesData;
    private CharacterTakeDamageController takeDamageController;

    [Header("Defense Inputs")]
    public bool blockInput;
    public bool deflectInput;

    [Header("Attack Inputs")]
    public bool normalAttackInput;
    public bool strikeAttackInput;

    [Header("Movement Inputs")]
    public bool toggleFlyInput;
    public bool flyUpInput;
    public bool flyDownInput;

    [Header("Movement Inputs")]
    public int damage;

    [ProButton]
    public void InputBlock() => blockInput = !blockInput;
    [ProButton]
    public void InputDeflect() => deflectInput = !deflectInput;
    [ProButton]
    public void InputNormalAttack() => normalAttackInput = !normalAttackInput;
    [ProButton]
    public void InputStrikeAttack() => strikeAttackInput = !strikeAttackInput;
    [ProButton]
    public void InputToggleFly() 
    {
        movementController.Jump();
        toggleFlyInput = !toggleFlyInput;
    }
    [ProButton]
    public void InputHit()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        takeDamageController.GetHit(null, damage, randomDirection, AttackTypes.normal_attack);
    }

    void Start()
    {
        if (character != null)
        {
            attackController = character.GetComponent<CharacterAttackController>();
            defenseController = character.GetComponent<CharacterDefenseController>();
            movementController = character.GetComponent<CharacterMovementController>();
            characterStatesData = character.GetComponent<CharacterStatesData>();
            takeDamageController = character.GetComponent<CharacterTakeDamageController>();

            movementController.endFlying += InputToggleFly;
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

        if (movementController != null)
        {
            if (toggleFlyInput)
            {
                movementController.SetFly(!characterStatesData.flyFlag);
                toggleFlyInput = false;
            }

            movementController.SetFlyUp(flyUpInput);
            movementController.SetFlyDown(flyDownInput);
        }
    }
}
