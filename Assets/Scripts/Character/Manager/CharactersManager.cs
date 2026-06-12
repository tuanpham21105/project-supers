using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CharacterActions
{
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    Sprint,
    Dash,
    ToggleFly,
    Jump,
    FlyUp,
    FlyDown,
    NormalAttack,
    StrikeAttack,
    Block,
    Deflect
}

public class CharactersManager : MonoBehaviour
{
    public static CharactersManager instance;

    [Header("Dependencies")]
    private MatchManager matchData;
    private PlayerData playerData;
    private HostPacketSender hostPeerConnectionSender;

    [Header("Data")]
    private List<GameObject> characters;
    private List<Action> flyingHandlers;
    private List<Action<String, String>> animationHandlers;

    [Header("Prefab")]
    [SerializeField] private GameObject characterPrefab;

    public event Action<String> onCharacterFlyingInterrupted;
    public event Action<String, float> onCharacterHealthChange;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        matchData = MatchManager.instance;
        playerData = PlayerData.instance;
        hostPeerConnectionSender = HostPacketSender.instance;

        characters = new List<GameObject>();
        flyingHandlers = new List<Action>();
        animationHandlers = new List<Action<String, String>>();

        onCharacterHealthChange += handleCharacterHealthChange;

        InitCharacters();
    }

    void InitCharacters()
    {
        for (int i = 0; i < matchData.GetPlayers().Count; i++)
        {
            String player = matchData.GetPlayers()[i];
            Transform spawnPoint = MapData.instance.getSpawnPoints()[i]; 

            GameObject character = Instantiate(characterPrefab, spawnPoint.position, Quaternion.identity);
            character.transform.LookAt(MapData.instance.transform);

            Action handler = HandleCharacterFlyingInterrupted(player);
            character.GetComponent<CharacterMovementController>().endFlying += handler;
            flyingHandlers.Add(handler);

            Action<String, String> animationHandler = HandleCharacterPlayAnimation(player);
            character.GetComponent<CharacterAnimationService>().onPlayAnimation += animationHandler;
            animationHandlers.Add(animationHandler);

            characters.Add(character);

            if (playerData.username.CompareTo(player) == 0) 
                CameraController.instance.SetCharacter(character.transform);
        }

        switchCharacterMode(MatchManager.instance.IsPlayerHost());
    }

    void FixedUpdate()
    {
        UpdateCharacterStates();
    }

    void OnDestroy()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i] != null)
                characters[i].GetComponent<CharacterMovementController>().endFlying -= flyingHandlers[i];
                characters[i].GetComponent<CharacterAnimationService>().onPlayAnimation -= animationHandlers[i];
        }
        
        onCharacterHealthChange -= handleCharacterHealthChange;
    }

    Action HandleCharacterFlyingInterrupted(String player)
    {
        return () => {
            if (HostPacketSender.instance != null) 
                HostPacketSender.instance.sendPlayerCharacterFlyingInterrupted(player);
            onCharacterFlyingInterrupted?.Invoke(player);
        };
    }

    Action<String, String> HandleCharacterPlayAnimation(String player)
    {
        return (type, animation) =>
        {
            if (HostPacketSender.instance != null) 
                hostPeerConnectionSender.sendPlayerCharacterAnimation(player, type, animation);
        };
    }

    void UpdateCharacterStates()
    {
        PlayersCharacterStatesDto data = new PlayersCharacterStatesDto();
        for (int i = 0; i < matchData.GetPlayers().Count; i++)
        {
            CharacterStatesDto states = new CharacterStatesDto();
            states.position = Vec3.From(characters[i].transform.position);
            states.forward = Vec3.From(characters[i].transform.forward);
            states.physicsColliderHeight = characters[i].GetComponent<CharacterController>().height;
            states.physicsColliderRadius = characters[i].GetComponent<CharacterController>().radius;

            CharacterStatesData statesData = characters[i].GetComponent<CharacterStatesData>();
            CharacterStatsData statsData = characters[i].GetComponent<CharacterStatsData>();
            
            states.currentProcessAction = statesData.currentProcessAction.ToString();

            states.moveFlag = statesData.moveFlag;
            states.jumpFlag = statesData.jumpFlag;
            states.sprintFlag = statesData.sprintFlag;
            states.dashFlag = statesData.dashFlag;
            states.dashCooldownFlag = statesData.dashCooldownFlag;
            states.flyFlag = statesData.flyFlag;
            states.flyUpFlag = statesData.flyUpFlag;
            states.flyDownFlag = statesData.flyDownFlag;
            states.fastFlyFlag = statesData.fastFlyFlag;
            states.attackFlag = statesData.attackFlag;
            states.normalAttackStartFlag = statesData.normalAttackStartFlag;
            states.normalAttackOngoingFlag = statesData.normalAttackOngoingFlag;
            states.normalAttackEndFlag = statesData.normalAttackEndFlag;
            states.strikeAttackStartFlag = statesData.strikeAttackStartFlag;
            states.strikeAttackOngoingFlag = statesData.strikeAttackOngoingFlag;
            states.strikeAttackEndFlag = statesData.strikeAttackEndFlag;
            states.knockAwayFlag = statesData.knockAwayFlag;
            states.blockFlag = statesData.blockFlag;
            states.deflectFlag = statesData.deflectFlag;
            states.upperActionFlag = statesData.upperActionFlag;
            states.bodyActionFlag = statesData.bodyActionFlag;
            states.hitFlag = statesData.hitFlag;
            states.deflectedFlag = statesData.deflectedFlag;
            states.deadFlag = statesData.deadFlag;
            states.fallFlag = statesData.fallFlag;

            states.currentEndurance = statesData.currentEndurance;
            states.moveSpeed = statesData.moveSpeed;

            states.inputAxes = Vec3.From(statesData.inputAxes);
            states.lookInput = Vec3.From(statesData.lookInput);
            states.direction = Vec3.From(statesData.direction);

            states.currentBodyAnimation = statesData.currentBodyAnimation.ToString();

            states.lastNormalAttackEndTime = statesData.lastNormalAttackEndTime;
            states.lastStrikeAttackEndTime = statesData.lastStrikeAttackEndTime;

            states.lastDeflectTime = statesData.lastDeflectTime;
            states.currentDeflectSpeed = statesData.currentDeflectSpeed;

            states.normalAttackComboIndex = statesData.normalAttackComboIndex;
            states.strikeAttackComboIndex = statesData.strikeAttackComboIndex;
            states.hitAnimationIndex = statesData.hitAnimationIndex;

            states.verticalVelocity = statesData.verticalVelocity;
            states.impactForce = Vec3.From(statesData.impactForce);
            states.dashForce = Vec3.From(statesData.dashForce);
            states.dashTimer = statesData.dashTimer;
            states.horizontalMove = Vec3.From(statesData.horizontalMove);
            states.isImpactActive = statesData.isImpactActive;
            states.currentMoveDirection = Vec3.From(statesData.currentMoveDirection);
            states.currentSqrMoveSpeed = statesData.currentSqrMoveSpeed;

            states.isFront = statesData.isFront;

            data.playersStates.Add(matchData.GetPlayers()[i], states);

            onCharacterHealthChange?.Invoke(matchData.GetPlayers()[i], (float)statesData.currentEndurance / (float)statsData.endurance);
        }

        if (HostPacketSender.instance != null) 
            hostPeerConnectionSender.sendPlayersCharacterStates(data);
    }

    public void ControlCharacterAction(String player, CharacterActions action, bool state)
    {
        if (!MatchManager.instance.IsMatchStart()) return;

        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterActionController controller = character.GetComponent<CharacterActionController>();

        switch (action)
        {
            case CharacterActions.MoveForward:
                controller.SetMoveForward(state);
                break;
            case CharacterActions.MoveBackward:
                controller.SetMoveBackward(state);
                break;
            case CharacterActions.MoveLeft:
                controller.SetMoveLeft(state);
                break;
            case CharacterActions.MoveRight:
                controller.SetMoveRight(state);
                break;
            case CharacterActions.Sprint:
                controller.SetSprint(state);
                break;
            case CharacterActions.Dash:
                controller.SetDash(state);
                break;
            case CharacterActions.ToggleFly:
                controller.SetToggleFly(state);
                break;
            case CharacterActions.Jump:
                controller.SetJump(state);
                break;
            case CharacterActions.FlyUp:
                controller.SetFlyUp(state);
                break;
            case CharacterActions.FlyDown:
                controller.SetFlyDown(state);
                break;
            case CharacterActions.NormalAttack:
                controller.SetNormalAttack(state);
                break;
            case CharacterActions.StrikeAttack:
                controller.SetStrikeAttack(state);
                break;
            case CharacterActions.Block:
                controller.SetBlock(state);
                break;
            case CharacterActions.Deflect:
                controller.SetDeflect(state);
                break;
        }
    }

    public void ControlCharacterRotation(String player, Vector3 direction)
    {
        if (!MatchManager.instance.IsMatchStart()) return;
        
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterActionController controller = character.GetComponent<CharacterActionController>();
        controller.SetRotation(direction);
    }

    //
    public void ControlCharacterAnimation(String player, String type, String name)
    {
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterSyncController controller = character.GetComponent<CharacterSyncController>();
        if (Enum.TryParse(type, true, out CharacterAnimationTypes animationType))
        {
            controller.PlayAnimation(animationType, name);
        }
    }

    public void ControlCharacterStates(String player, CharacterStatesDto characterStatesDTO)
    {
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterSyncController controller = character.GetComponent<CharacterSyncController>();

        controller.ApplyTransform(characterStatesDTO.position.ToVector3(), characterStatesDTO.forward.ToVector3());
        controller.ApplyPhysicsCollider(characterStatesDTO.physicsColliderRadius, characterStatesDTO.physicsColliderHeight);
        controller.ApplyStates(characterStatesDTO);

        onCharacterHealthChange?.Invoke(player, (float)characterStatesDTO.currentEndurance / (float)character.GetComponent<CharacterStatsData>().endurance);
    }

    // Switch Character mode
    public void switchCharacterMode(bool isHost)
    {
        foreach (GameObject character in characters)
        {
            character.GetComponent<CharacterController>().enabled = isHost;
            character.GetComponent<CharacterObjectService>().enabled = isHost;
            character.GetComponent<CharacterMovementController>().enabled = isHost;
            character.GetComponent<CharacterAttackController>().enabled = isHost;
            character.GetComponent<CharacterDefenseController>().enabled = isHost;
            character.GetComponent<CharacterDebuffController>().enabled = isHost;
            character.GetComponent<CharacterTakeDamageController>().enabled = isHost;
            character.GetComponent<CharacterActionController>().enabled = isHost;
            character.GetComponent<CharacterObjectsData>().characterHurtBox.SetActive(isHost);
        }
    }

    void handleCharacterHealthChange(String player, float healthPercent)
    {
        if (healthPercent <= 0)
        {
            foreach (String p in MatchManager.instance.GetPlayers())
            {
                if (!player.Equals(p))
                {
                    MatchFinishManager.instance.Finish(p);
                }
            }
        }
    }
}
