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
    private MatchData matchData;
    private PlayerData playerData;

    [Header("Data")]
    private List<GameObject> characters;
    private List<Action> flyingHandlers;

    [Header("Prefabs")]
    [SerializeField] private GameObject hostCharacterPrefab;

    public event Action<String> onCharacterFlyingInterrupted;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        matchData = GetComponent<MatchData>();
        playerData = GetComponent<PlayerData>();

        characters = new List<GameObject>();
        flyingHandlers = new List<Action>();

        if (matchData.IsPlayerHost(playerData.player))
        {
            foreach (String player in matchData.GetPlayers())
            {
                Vector3 randomPos = new Vector3(Random.Range(-30f, 30f), 1.5f, Random.Range(30f, 30f));
                GameObject character = Instantiate(hostCharacterPrefab, randomPos, Quaternion.identity);
                Action handler = HandleCharacterFlyingInterrupted(player);
                character.GetComponent<CharacterMovementController>().endFlying += handler;
                flyingHandlers.Add(handler);
                characters.Add(character);

                if (matchData.IsPlayerHost(player)) 
                    CameraController.instance.SetCharacter(character.transform);
            }
        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            characters[i].GetComponent<CharacterMovementController>().endFlying -= flyingHandlers[i];
        }
    }

    Action HandleCharacterFlyingInterrupted(String player)
    {
        return () => onCharacterFlyingInterrupted?.Invoke(player);
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
}
