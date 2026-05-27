using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClientCharactersManager : MonoBehaviour
{
    public static ClientCharactersManager instance;

    [Header("Dependencies")]
    private MatchManager matchData;
    private PlayerData playerData;

    [Header("Data")]
    private List<GameObject> characters;

    [Header("Prefab")]
    [SerializeField] private GameObject clientCharacterPrefab;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        matchData = MatchManager.instance;
        playerData = PlayerData.instance;

        characters = new List<GameObject>();

        foreach (String player in matchData.GetPlayers())
        {
            Vector3 randomPos = new Vector3(Random.Range(-30f, 30f), 1.5f, Random.Range(30f, 30f));
            GameObject character = Instantiate(clientCharacterPrefab, randomPos, Quaternion.identity);
            characters.Add(character);

            if (playerData.player.CompareTo(player) == 0) 
                CameraController.instance.SetCharacter(character.transform);
        }
    }

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

        controller.ApplyTransform(characterStatesDTO.position, characterStatesDTO.rotation);
        controller.ApplyPhysicsCollider(characterStatesDTO.physicsColliderRadius, characterStatesDTO.physicsColliderHeight);
    }
}
