using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyboadAndMouseInputController : PlayerInputController
{
    private PlayerKeyboardAndMouseKeybindsData keybinds;
    private IPlayerInputHandler playerInputHandler;
    private PlayerData playerData;

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
    public bool deflectInput;
    public float verticalRotationInput;
    public float horizontalRotationInput;

    public bool _forwardInput;
    public bool _backwardInput;
    public bool _leftInput;
    public bool _rightInput;

    private Dictionary<String, float> lastClickTime = new Dictionary<String, float>();
    [SerializeField] private float doubleClickThreshold = 0.3f;

    // Hold: key must be held at least this long before activating
    [SerializeField] private float holdThreshold = 0.2f;
    private Dictionary<String, float> holdStartTime = new Dictionary<String, float>();

    private void Awake()
    {
        PlayerInputController.instance = this;
    }

    private void Start()
    {
        playerInputHandler = IPlayerInputHandler.instance;
        playerData = PlayerData.instance;

        playerInputHandler.playerInputController = this;

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
        bool prevForward = _forwardInput;
        bool prevBackward = _backwardInput;
        bool prevLeft = _leftInput;
        bool prevRight = _rightInput;

        _forwardInput = ProcessInput(keybinds.forwardKey, _forwardInput);
        _backwardInput = ProcessInput(keybinds.backwardKey, _backwardInput);
        _leftInput = ProcessInput(keybinds.strafeLeftKey, _leftInput);
        _rightInput = ProcessInput(keybinds.strafeRightKey, _rightInput);

        // if (_forwardInput != prevForward)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveForward, _forwardInput);
        // if (_backwardInput != prevBackward)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveBackward, _backwardInput);
        // if (_leftInput != prevLeft)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveLeft, _leftInput);
        // if (_rightInput != prevRight)
            playerInputHandler.ControlCharacterAction(CharacterActions.MoveRight, _rightInput);

        HandleOnceActivation(ref _forwardInput, keybinds.forwardKey);
        HandleOnceActivation(ref _backwardInput, keybinds.backwardKey);
        HandleOnceActivation(ref _leftInput, keybinds.strafeLeftKey);
        HandleOnceActivation(ref _rightInput, keybinds.strafeRightKey);
    }

    public override void SprintInput()
    {
        bool currentSprintInput = ProcessInput(keybinds.sprintKey, sprintInput);

        sprintInput = currentSprintInput;

        playerInputHandler.ControlCharacterAction(CharacterActions.Sprint, sprintInput);

        HandleOnceActivation(ref sprintInput, keybinds.sprintKey);
    }

    public override void DashInput()
    {
        dashInput = ProcessInput(keybinds.dashKey, dashInput);
        if (dashInput)
        {
            playerInputHandler.ControlCharacterAction(CharacterActions.Dash, true);
            HandleOnceActivation(ref dashInput, keybinds.dashKey);
        }
    }

    public override void ToggleFlyInput()
    {
        bool currentFlyInput = ProcessInput(keybinds.toggleFlyKey, toggleFlyInput);

        toggleFlyInput = currentFlyInput;

        playerInputHandler.ControlCharacterAction(CharacterActions.ToggleFly, toggleFlyInput);

        HandleOnceActivation(ref toggleFlyInput, keybinds.toggleFlyKey);
    }

    public override void JumpInput()
    {
        jumpInput = ProcessInput(keybinds.jumpKey, jumpInput);
        if (jumpInput)
        {
            playerInputHandler.ControlCharacterAction(CharacterActions.Jump, true);
            HandleOnceActivation(ref jumpInput, keybinds.jumpKey);
        }
    }

    public override void FlyUpInput()
    {
        bool currentInput = ProcessInput(keybinds.flyUpKey, flyUpInput);
        
        flyUpInput = currentInput;

        playerInputHandler.ControlCharacterAction(CharacterActions.FlyUp, flyUpInput);

        HandleOnceActivation(ref flyUpInput, keybinds.flyUpKey);
    }

    public override void FlyDownInput()
    {
        bool currentInput = ProcessInput(keybinds.flyDownKey, flyDownInput);
        
        flyDownInput = currentInput;

        playerInputHandler.ControlCharacterAction(CharacterActions.FlyDown, flyDownInput);

        HandleOnceActivation(ref flyDownInput, keybinds.flyDownKey);
    }

    public override void NormalAttackInput()
    {
        normalAttackInput = ProcessInput(keybinds.normalAttackKey, normalAttackInput);
        if (normalAttackInput)
        {
            playerInputHandler.ControlCharacterAction(CharacterActions.NormalAttack, true);
            HandleOnceActivation(ref normalAttackInput, keybinds.normalAttackKey);
        }
    }

    public override void StrikeAttackInput()
    {
        strikeAttackInput = ProcessInput(keybinds.strikeAttackKey, strikeAttackInput);
        if (strikeAttackInput)
        {
            playerInputHandler.ControlCharacterAction(CharacterActions.StrikeAttack, true);
            HandleOnceActivation(ref strikeAttackInput, keybinds.strikeAttackKey);
        }
    }

    public override void BlockInput()
    {
        bool currentInput = ProcessInput(keybinds.blockKey, blockInput);
        blockInput = currentInput;

        playerInputHandler.ControlCharacterAction(CharacterActions.Block, blockInput);

        HandleOnceActivation(ref blockInput, keybinds.blockKey);
    }

    public override void DeflectInput()
    {
        deflectInput = ProcessInput(keybinds.deflectKey, deflectInput);
        if (deflectInput)
        {
            playerInputHandler.ControlCharacterAction(CharacterActions.Deflect, true);
            HandleOnceActivation(ref deflectInput, keybinds.deflectKey);
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

    private void HandleOnceActivation(ref bool inputState, Keybind keybind)
    {
        if (inputState && (keybind.action == ActivateAction.Once || keybind.action == ActivateAction.Any))
        {
            inputState = false;
        }
    }
}
