using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboadAndMouseInputController : PlayerInputController
{
    private PlayerKeyboardAndMouseKeybindsData keybinds;
    [SerializeField] private CharacterMovementController characterMovementController;
    [SerializeField] private CharacterAttackController characterAttackController;
    [SerializeField] private CharacterDefenseController characterDefenseController;
    [SerializeField] private CameraController cameraController;

    public Vector2 moveDirectionInput;
    public bool sprintInput;
    public bool dashInput;
    public bool toggleFlyInput;
    public bool jumpInput;
    public bool flyUpInput;
    public bool flyDownInput;
    public bool normalAttackInput;
    public bool strikeAttackInput;
    public bool blockInput;
    public bool parryInput;
    public float verticalRotationInput;
    public float horizontalRotationInput;

    public bool _forwardInput;
    public bool _backwardInput;
    public bool _leftInput;
    public bool _rightInput;

    private Dictionary<KeyCode, float> lastClickTime = new Dictionary<KeyCode, float>();
    public const float doubleClickThreshold = 0.3f;

    private void Awake()
    {
        PlayerInputController.instance = this;
    }

    private void Start()
    {
        keybinds = PlayerKeyboardAndMouseKeybindsData.playerKeyboardAndMouseKeybindsData;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private Vector2 smoothLookInput;
    [SerializeField] private float lookSmoothing = 0.1f;

    private void Update()
    {
        MoveDirectionInput();
        SprintInput();
        DashInput();
        ToggleFlyInput();
        JumpInput();
        FlyUpInput();
        FlyDownInput();
        NormalAttackInput();
        RotationInput();
        StrikeAttackInput();
        ParryInput();
        BlockInput();
    }

    private void FixedUpdate()
    {
        // Physics-related logic remains here if needed, 
        // but current implementation handles movement via Move calls triggered by inputs.
    }

    public override void MoveDirectionInput()
    {
        _forwardInput = ProcessInput(keybinds.forwardKey, _forwardInput);
        _backwardInput = ProcessInput(keybinds.backwardKey, _backwardInput);
        _leftInput = ProcessInput(keybinds.strafeLeftKey, _leftInput);
        _rightInput = ProcessInput(keybinds.strafeRightKey, _rightInput);

        float x = 0;
        float y = 0;

        if (_forwardInput) y += 1;
        if (_backwardInput) y -= 1;
        if (_leftInput) x -= 1;
        if (_rightInput) x += 1;

        Vector2 currentMoveInput = new Vector2(x, y).normalized;
        if (currentMoveInput != moveDirectionInput)
        {
            moveDirectionInput = currentMoveInput;

            characterMovementController.Move(moveDirectionInput);
        }

        HandleOnceActivation(ref _forwardInput, keybinds.forwardKey);
        HandleOnceActivation(ref _backwardInput, keybinds.backwardKey);
        HandleOnceActivation(ref _leftInput, keybinds.strafeLeftKey);
        HandleOnceActivation(ref _rightInput, keybinds.strafeRightKey);
    }

    public override void SprintInput()
    {
        bool currentSprintInput = ProcessInput(keybinds.sprintKey, sprintInput);
        if (currentSprintInput != sprintInput)
        {
            sprintInput = currentSprintInput;

            characterMovementController.Sprint(sprintInput);
        }

        HandleOnceActivation(ref sprintInput, keybinds.sprintKey);
    }

    public override void DashInput()
    {
        dashInput = ProcessInput(keybinds.dashKey, dashInput);
        if (dashInput)
        {
            characterMovementController.Dash();
            HandleOnceActivation(ref dashInput, keybinds.dashKey);
        }
    }

    public override void ToggleFlyInput()
    {
        bool currentFlyInput = ProcessInput(keybinds.toggleFlyKey, toggleFlyInput);
        if (currentFlyInput != toggleFlyInput)
        {
            toggleFlyInput = currentFlyInput;

            characterMovementController.ToggleFly(toggleFlyInput);
        }

        HandleOnceActivation(ref toggleFlyInput, keybinds.toggleFlyKey);
    }

    public override void JumpInput()
    {
        jumpInput = ProcessInput(keybinds.jumpKey, jumpInput);
        if (jumpInput)
        {
            characterMovementController.Jump();
            HandleOnceActivation(ref jumpInput, keybinds.jumpKey);
        }
    }

    public override void FlyUpInput()
    {
        bool currentInput = ProcessInput(keybinds.flyUpKey, flyUpInput);
        if (currentInput != flyUpInput)
        {
            flyUpInput = currentInput;

            characterMovementController.FlyUp(flyUpInput);
        }

        HandleOnceActivation(ref flyUpInput, keybinds.flyUpKey);
    }

    public override void FlyDownInput()
    {
        bool currentInput = ProcessInput(keybinds.flyDownKey, flyDownInput);
        if (currentInput != flyDownInput)
        {
            flyDownInput = currentInput;

            characterMovementController.FlyDown(flyDownInput);
        }

        HandleOnceActivation(ref flyDownInput, keybinds.flyDownKey);
    }

    public override void NormalAttackInput()
    {
        normalAttackInput = ProcessInput(keybinds.normalAttackKey, normalAttackInput);
        if (normalAttackInput)
        {
            characterAttackController.StartNormalAttack();
            HandleOnceActivation(ref normalAttackInput, keybinds.normalAttackKey);
        }
    }

    public override void StrikeAttackInput()
    {
        strikeAttackInput = ProcessInput(keybinds.strikeAttackKey, strikeAttackInput);
        if (strikeAttackInput)
        {
            characterAttackController.StartStrikeAttack();
            HandleOnceActivation(ref strikeAttackInput, keybinds.strikeAttackKey);
        }
    }

    public override void BlockInput()
    {
        bool currentInput = ProcessInput(keybinds.blockKey, blockInput);
        if (currentInput != blockInput)
        {
            blockInput = currentInput;
            if (characterDefenseController != null) characterDefenseController.Block(blockInput);
        }
        HandleOnceActivation(ref blockInput, keybinds.blockKey);
    }

    public override void ParryInput()
    {
        parryInput = ProcessInput(keybinds.parryKey, parryInput);
        if (parryInput)
        {
            // Placeholder for Parry Action
            // characterAttackController.Parry();
            HandleOnceActivation(ref parryInput, keybinds.parryKey);
        }
    }

    public override void RotationInput()
    {
        float targetMouseX = Input.GetAxis("Mouse X") * keybinds.mouseSensitivity;
        float targetMouseY = Input.GetAxis("Mouse Y") * keybinds.mouseSensitivity;

        // Apply smoothing (Lerp) to the mouse delta
        smoothLookInput.x = Mathf.Lerp(smoothLookInput.x, targetMouseX, 1f / lookSmoothing * Time.deltaTime);
        smoothLookInput.y = Mathf.Lerp(smoothLookInput.y, targetMouseY, 1f / lookSmoothing * Time.deltaTime);

        if (cameraController != null) cameraController.Rotate(smoothLookInput);
        characterMovementController.Rotate(cameraController.GetCameraDirection());
    }

    private bool ProcessInput(Keybind keybind, bool currentState)
    {
        bool triggered = false;
        if (keybind.mode == KeyMode.Click) triggered = Input.GetKeyDown(keybind.key);
        else if (keybind.mode == KeyMode.DoubleClick) triggered = CheckDoubleClick(keybind.key);

        if (keybind.action == ActivateAction.Toggle)
        {
            if (triggered) return !currentState;
            return currentState;
        }
        else if (keybind.action == ActivateAction.Once)
        {
            return triggered;
        }
        else // Hold
        {
            if (triggered) return true;
            if (currentState)
            {
                // Stay true as long as key is held
                return Input.GetKey(keybind.key);
            }
            return false;
        }
    }

    private bool CheckDoubleClick(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            float timeSinceLastClick = Time.time - lastClickTime.GetValueOrDefault(key, -10f);
            lastClickTime[key] = Time.time;
            if (timeSinceLastClick <= doubleClickThreshold)
            {
                return true;
            }
        }
        return false;
    }

    private void HandleOnceActivation(ref bool inputState, Keybind keybind)
    {
        if (inputState && keybind.action == ActivateAction.Once)
        {
            inputState = false;
        }
    }
}
