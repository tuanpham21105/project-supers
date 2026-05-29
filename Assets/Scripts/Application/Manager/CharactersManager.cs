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

        foreach (String player in matchData.GetPlayers())
        {
            Vector3 randomPos = new Vector3(Random.Range(-30f, 30f), 1.5f, Random.Range(30f, 30f));
            GameObject character = Instantiate(characterPrefab, randomPos, Quaternion.identity);

            Action handler = HandleCharacterFlyingInterrupted(player);
            character.GetComponent<CharacterMovementController>().endFlying += handler;
            flyingHandlers.Add(handler);

            Action<String, String> animationHandler = HandleCharacterPlayAnimation(player);
            character.GetComponent<CharacterAnimationController>().onPlayAnimation += animationHandler;
            animationHandlers.Add(animationHandler);

            characters.Add(character);

            if (playerData.player.CompareTo(player) == 0) 
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
                characters[i].GetComponent<CharacterAnimationController>().onPlayAnimation -= animationHandlers[i];
        }
    }

    Action HandleCharacterFlyingInterrupted(String player)
    {
        return () => {
            HostPacketSender.instance.sendPlayerCharacterFlyingInterrupted(player);
            onCharacterFlyingInterrupted?.Invoke(player);
        };
    }

    Action<String, String> HandleCharacterPlayAnimation(String player)
    {
        return (type, animation) =>
        {
            hostPeerConnectionSender.sendPlayerCharacterAnimation(player, type, animation);
        };
    }

    void UpdateCharacterStates()
    {
        PlayersCharacterStatesDto data = new PlayersCharacterStatesDto();
        for (int i = 0; i < matchData.GetPlayers().Count; i++)
        {
            CharacterStatesDTO states = new CharacterStatesDTO();
            states.position = Vec3.From(characters[i].transform.position);
            states.forward = Vec3.From(characters[i].transform.forward);
            states.physicsColliderHeight = characters[i].GetComponent<CharacterController>().height;
            states.physicsColliderRadius = characters[i].GetComponent<CharacterController>().radius;
            data.playersStates.Add(matchData.GetPlayers()[i], states);
        }
        hostPeerConnectionSender.sendPlayersCharacterStates(data);
    }

    public void ControlCharacterAction(String player, CharacterActions action, bool state)
    {
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterActionController controller = character.GetComponent<CharacterActionController>();

        switch (action)
        {
            case CharacterActions.MoveForward:
                controller.MoveForward(state);
                break;
            case CharacterActions.MoveBackward:
                controller.MoveBackward(state);
                break;
            case CharacterActions.MoveLeft:
                controller.MoveLeft(state);
                break;
            case CharacterActions.MoveRight:
                controller.MoveRight(state);
                break;
            case CharacterActions.Sprint:
                controller.Sprint(state);
                break;
            case CharacterActions.Dash:
                controller.Dash(state);
                break;
            case CharacterActions.ToggleFly:
                controller.ToggleFly(state);
                break;
            case CharacterActions.Jump:
                controller.Jump(state);
                break;
            case CharacterActions.FlyUp:
                controller.FlyUp(state);
                break;
            case CharacterActions.FlyDown:
                controller.FlyDown(state);
                break;
            case CharacterActions.NormalAttack:
                controller.NormalAttack(state);
                break;
            case CharacterActions.StrikeAttack:
                controller.StrikeAttack(state);
                break;
            case CharacterActions.Block:
                controller.Block(state);
                break;
            case CharacterActions.Deflect:
                controller.Deflect(state);
                break;
        }
    }

    public void ControlCharacterRotation(String player, Vector3 direction)
    {
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterActionController controller = character.GetComponent<CharacterActionController>();
        controller.Rotation(direction);
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

    public void ControlCharacterStates(String player, CharacterStatesDTO characterStatesDTO)
    {
        int index = matchData.GetPlayerIndex(player);
        GameObject character = characters[index];
        CharacterSyncController controller = character.GetComponent<CharacterSyncController>();

        controller.ApplyTransform(characterStatesDTO.position.ToVector3(), characterStatesDTO.forward.ToVector3());
        controller.ApplyPhysicsCollider(characterStatesDTO.physicsColliderRadius, characterStatesDTO.physicsColliderHeight);
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
}
