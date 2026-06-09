using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboadAndMouseInputController : PlayerInputController
{
    private PlayerKeyboardAndMouseKeybindsData keybinds;
    private PlayerInputHandler playerInputHandler;
    private PlayerData playerData;

    [SerializeField] private CameraController cameraController;

    public bool prevSprintInput;
    public bool prevDashInput;
    public bool prevToggleFlyInput;
    public bool prevJumpInput;
    public bool prevFlyUpInput;
    public bool prevFlyDownInput;
    public bool prevNormalAttackInput;
    public bool prevStrikeAttackInput;
    public bool prevBlockInput;
    public bool prevDeflectInput;
    public bool prevForwardInput;
    public bool prevBackwardInput;
    public bool prevLeftInput;
    public bool prevRightInput;

    public bool sprintInput;
    public bool dashInput;
    public bool toggleFlyInput;
    public bool jumpInput;
    public bool flyUpInput;
    public bool flyDownInput;
    public bool normalAttackInput;
    public bool strikeAttackInput;
    public bool blockInput;
    public bool deflectInput;
    public float verticalRotationInput;
    public float horizontalRotationInput;
    public bool forwardInput;
    public bool backwardInput;
    public bool leftInput;
    public bool rightInput;

    private Dictionary<String, float> lastClickTime = new Dictionary<String, float>();
    [SerializeField] private float doubleClickThreshold = 0.3f;

    // Hold: key must be held at least this long before activating
    [SerializeField] private float holdThreshold = 0.2f;
    private Dictionary<String, float> holdStartTime = new Dictionary<String, float>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerInputHandler = PlayerInputHandler.instance;
        playerData = PlayerData.instance;

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
        DeflectInput();
        BlockInput();
    }

    // private void FixedUpdate()
    // {
    //     // Physics-related logic remains here if needed, 
    //     // but current implementation handles movement via Move calls triggered by inputs.
    // }

    public override void HandleFlyingInterrupted()
    {
        toggleFlyInput = false;
    }

    public override void MoveDirectionInput()
    {
        prevForwardInput = forwardInput;
        prevBackwardInput = backwardInput;
        prevLeftInput = leftInput;
        prevRightInput = rightInput;

        forwardInput = ProcessInput(keybinds.forwardKey, forwardInput);
        backwardInput = ProcessInput(keybinds.backwardKey, backwardInput);
        leftInput = ProcessInput(keybinds.strafeLeftKey, leftInput);
        rightInput = ProcessInput(keybinds.strafeRightKey, rightInput);

        if (forwardInput != prevForwardInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveForward, forwardInput);
        if (backwardInput != prevBackwardInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveBackward, backwardInput);
        if (leftInput != prevLeftInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveLeft, leftInput);
        if (rightInput != prevRightInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveRight, rightInput);

        // StartCoroutine(HandleOnceActivation(prevForwardInput, forwardInput, keybinds.forwardKey, CharacterActions.MoveForward));
        // StartCoroutine(HandleOnceActivation(prevBackwardInput, backwardInput, keybinds.backwardKey, CharacterActions.MoveBackward));
        // StartCoroutine(HandleOnceActivation(prevLeftInput, leftInput, keybinds.strafeLeftKey, CharacterActions.MoveLeft));
        // StartCoroutine(HandleOnceActivation(prevRightInput, rightInput, keybinds.strafeRightKey, CharacterActions.MoveRight));
    }

    public override void SprintInput()
    {
        prevSprintInput = sprintInput;
        bool currentSprintInput = ProcessInput(keybinds.sprintKey, sprintInput);

        sprintInput = currentSprintInput;

        if (sprintInput != prevSprintInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.Sprint, sprintInput);

        // StartCoroutine(HandleOnceActivation(prevSprintInput, sprintInput, keybinds.sprintKey, CharacterActions.Sprint));
    }

    public override void DashInput()
    {
        prevDashInput = dashInput;
        dashInput = ProcessInput(keybinds.dashKey, dashInput);

        if (dashInput != prevDashInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.Dash, dashInput);

        // StartCoroutine(HandleOnceActivation(prevDashInput, dashInput, keybinds.dashKey, CharacterActions.Dash));
    }

    public override void ToggleFlyInput()
    {
        prevToggleFlyInput = toggleFlyInput;
        toggleFlyInput = ProcessInput(keybinds.toggleFlyKey, toggleFlyInput);

        if (toggleFlyInput != prevToggleFlyInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.ToggleFly, toggleFlyInput);

        // StartCoroutine(HandleOnceActivation(prevToggleFlyInput, toggleFlyInput, keybinds.toggleFlyKey, CharacterActions.ToggleFly));
    }

    public override void JumpInput()
    {
        prevJumpInput = jumpInput;
        jumpInput = ProcessInput(keybinds.jumpKey, jumpInput);

        if (jumpInput != prevJumpInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.Jump, jumpInput);

        // StartCoroutine(HandleOnceActivation(prevJumpInput, jumpInput, keybinds.jumpKey, CharacterActions.Jump));
    }

    public override void FlyUpInput()
    {
        prevFlyUpInput = flyUpInput;
        flyUpInput = ProcessInput(keybinds.flyUpKey, flyUpInput);

        if (flyUpInput != prevFlyUpInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.FlyUp, flyUpInput);

        // StartCoroutine(HandleOnceActivation(prevFlyUpInput, flyUpInput, keybinds.flyUpKey, CharacterActions.FlyUp));
    }

    public override void FlyDownInput()
    {
        prevFlyDownInput = flyDownInput;
        flyDownInput = ProcessInput(keybinds.flyDownKey, flyDownInput);

        if (flyDownInput != prevFlyDownInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.FlyDown, flyDownInput);

        // StartCoroutine(HandleOnceActivation(prevFlyDownInput, flyDownInput, keybinds.flyDownKey, CharacterActions.FlyDown));
    }

    public override void NormalAttackInput()
    {
        prevNormalAttackInput = normalAttackInput;
        normalAttackInput = ProcessInput(keybinds.normalAttackKey, normalAttackInput);

        if (normalAttackInput != prevNormalAttackInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.NormalAttack, normalAttackInput);

        // StartCoroutine(HandleOnceActivation(prevNormalAttackInput, normalAttackInput, keybinds.normalAttackKey, CharacterActions.NormalAttack));
    }

    public override void StrikeAttackInput()
    {
        prevStrikeAttackInput = strikeAttackInput;
        strikeAttackInput = ProcessInput(keybinds.strikeAttackKey, strikeAttackInput);

        if (strikeAttackInput != prevStrikeAttackInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.StrikeAttack, strikeAttackInput);

        // StartCoroutine(HandleOnceActivation(prevStrikeAttackInput, strikeAttackInput, keybinds.strikeAttackKey, CharacterActions.StrikeAttack));
    }

    public override void BlockInput()
    {
        prevBlockInput = blockInput;
        blockInput = ProcessInput(keybinds.blockKey, blockInput);

        if (blockInput != prevBlockInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.Block, blockInput);

        // StartCoroutine(HandleOnceActivation(prevBlockInput, blockInput, keybinds.blockKey, CharacterActions.Block));
    }

    public override void DeflectInput()
    {
        prevDeflectInput = deflectInput;
        deflectInput = ProcessInput(keybinds.deflectKey, deflectInput);

        if (deflectInput != prevDeflectInput)
            playerInputHandler.ControlCharacterAction(CharacterActions.Deflect, deflectInput);
            
        // StartCoroutine(HandleOnceActivation(prevDeflectInput, deflectInput, keybinds.deflectKey, CharacterActions.Deflect));
    }

    public override void RotationInput()
    {
        float targetMouseX = Input.GetAxis("Mouse X") * keybinds.mouseSensitivity;
        float targetMouseY = Input.GetAxis("Mouse Y") * keybinds.mouseSensitivity;

        // Apply smoothing (Lerp) to the mouse delta
        smoothLookInput.x = Mathf.Lerp(smoothLookInput.x, targetMouseX, 1f / lookSmoothing * Time.deltaTime);
        smoothLookInput.y = Mathf.Lerp(smoothLookInput.y, targetMouseY, 1f / lookSmoothing * Time.deltaTime);

        if (cameraController != null) cameraController.Rotate(smoothLookInput);
        playerInputHandler.ControlCharacterRotation(cameraController.GetCameraDirection());
    }

    private bool ProcessInput(Keybind keybind, bool currentState)
    {
        KeyCode key = keybind.key;
        String name = keybind.name;
        bool keyIsDown = Input.GetKey(key);
        bool keyJustDown = Input.GetKeyDown(key);
        bool keyJustUp = Input.GetKeyUp(key);

        // Determine trigger based on KeyMode
        bool triggered = false;
        if (keybind.mode == KeyMode.Click) triggered = keyJustDown;
        else if (keybind.mode == KeyMode.DoubleClick) triggered = CheckDoubleClick(name, keyJustDown);

        // Record start time on trigger
        if (triggered)
        {
            holdStartTime[name] = Time.time;
        }

        if (keybind.action == ActivateAction.Any)
        {
            return keyIsDown;
        }

        // On KeyUp: Handle Once/Toggle/Any(tap) if duration was short
        if (keyJustUp && holdStartTime.ContainsKey(name))
        {
            float duration = Time.time - holdStartTime[name];
            holdStartTime.Remove(name);

            if (duration < holdThreshold)
            {
                if (keybind.action == ActivateAction.Toggle) return !currentState;
                if (keybind.action == ActivateAction.Once)   return true;
                if (keybind.action == ActivateAction.Any)   return true;
            }
        }

        // While Holding: Handle Hold/Any(hold) if threshold met
        if (keyIsDown && holdStartTime.ContainsKey(name))
        {
            float duration = Time.time - holdStartTime[name];
            if (duration >= holdThreshold)
            {
                if (keybind.action == ActivateAction.Hold) return true;
                if (keybind.action == ActivateAction.Any)   return true;
            }
        }

        return (keybind.action == ActivateAction.Toggle) ? currentState : false;
    }

    private bool CheckDoubleClick(String name, bool keyJustDown)
    {
        if (keyJustDown)
        {
            float timeSinceLastClick = Time.time - lastClickTime.GetValueOrDefault(name, -10f);
            lastClickTime[name] = Time.time;
            if (timeSinceLastClick <= doubleClickThreshold)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator HandleOnceActivation(bool prevInputState, bool inputState, Keybind keybind, CharacterActions action)
    {
        if (!prevInputState && inputState && keybind.action == ActivateAction.Once)
        {
            yield return new WaitForFixedUpdate();

            playerInputHandler.ControlCharacterAction(action, false);
        }
    }
}
