using System;
using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

/// <summary>
/// Quản lý CHỈ 1 character của người chơi trong Training Area.
/// Không kết nối online, không gửi packet, không phụ thuộc MatchManager.
/// Tái sử dụng enum CharacterActions từ CharactersManager.cs.
/// </summary>
public class TrainingCharacterManager : MonoBehaviour
{
    public static TrainingCharacterManager instance;

    [Header("Dependencies")]
    private PlayerData playerData;

    [Header("Data")]
    private GameObject character;
    private Action flyingHandler;
    private Action<int, bool> getHitHandler;
    private Action dashStartHandler;
    private Action<float> dashCooldownStartHandler;
    private Action dashCooldownEndHandler;

    [Header("Spawn")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform spawnPoint;

    // ─────────────────────────────────────────────
    // Events — vẫn giữ để UI/hệ thống khác lắng nghe
    // ─────────────────────────────────────────────
    public event Action onCharacterFlyingInterrupted;
    public event Action<float> onCharacterHealthChange; // healthPercent 0-1
    public event Action onCharacterDashStart;
    public event Action<float> onCharacterDashCooldownStart;
    public event Action onCharacterDashCooldownEnd;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerData = PlayerData.instance;

        InitCharacter();
    }

    void InitCharacter()
    {
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : Vector3.zero;

        character = Instantiate(characterPrefab, spawnPos, Quaternion.identity);
        character.transform.LookAt(character.transform.position + Vector3.forward);

        flyingHandler = HandleCharacterFlyingInterrupted();
        character.GetComponent<CharacterActionController>().onFlyingInterrupted += flyingHandler;

        getHitHandler = HandleCharacterGetHit();
        character.GetComponent<CharacterTakeDamageController>().onGetHit += getHitHandler;

        dashStartHandler = HandleDashStart();
        character.GetComponent<CharacterActionController>().onDashStart += dashStartHandler;

        dashCooldownStartHandler = HandleDashCooldownStart();
        character.GetComponent<CharacterActionController>().onDashCooldownStart += dashCooldownStartHandler;

        dashCooldownEndHandler = HandleDashCooldownEnd();
        character.GetComponent<CharacterActionController>().onDashCooldownEnd += dashCooldownEndHandler;

        CameraController.instance.SetCharacter(character.transform);
        ApplyCharacterCustomize(playerData.characterCustomizies);
        ApplyCharacterAccessories(playerData.characterAccessories);
        ApplyEmblem(character.GetComponent<CharacterAccessoriesController>(), PlayerData.instance.emblem);
        character.GetComponent<CharacterObjectsData>().targetVfx.SetActive(false);

        // Không có khái niệm host/client — character luôn được điều khiển đầy đủ
        EnableCharacterControllers(true);
    }

    void FixedUpdate()
    {
        // Không cần build & gửi PlayersCharacterStatesDto — không có ai để đồng bộ tới
    }

    void OnDestroy()
    {
        instance = null;

        if (character != null)
        {
            character.GetComponent<CharacterActionController>().onFlyingInterrupted -= flyingHandler;
            character.GetComponent<CharacterTakeDamageController>().onGetHit -= getHitHandler;
            character.GetComponent<CharacterActionController>().onDashStart -= dashStartHandler;
            character.GetComponent<CharacterActionController>().onDashCooldownStart -= dashCooldownStartHandler;
            character.GetComponent<CharacterActionController>().onDashCooldownEnd -= dashCooldownEndHandler;
        }
    }

    Action HandleCharacterFlyingInterrupted()
    {
        return () =>
        {
            onCharacterFlyingInterrupted?.Invoke();
        };
    }

    Action HandleDashStart()
    {
        return () =>
        {
            onCharacterDashStart?.Invoke();
        };
    }

    Action<float> HandleDashCooldownStart()
    {
        return (duration) =>
        {
            onCharacterDashCooldownStart?.Invoke(duration);
        };
    }

    Action HandleDashCooldownEnd()
    {
        return () =>
        {
            onCharacterDashCooldownEnd?.Invoke();
        };
    }

    // ─────────────────────────────────────────────
    // Control — bỏ check MatchManager.IsMatchStart(), không có player param vì chỉ 1 character
    // ─────────────────────────────────────────────

    public void ControlCharacterAction(CharacterActions action, bool state)
    {
        if (character == null) return;

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

    public void ControlCharacterRotation(Vector3 direction)
    {
        if (character == null) return;

        CharacterActionController controller = character.GetComponent<CharacterActionController>();
        controller.SetRotation(direction);
    }

    // ControlCharacterAnimation và ControlCharacterStates KHÔNG cần —
    // đó là hàm đồng bộ character ở phía Client nhận dữ liệu từ Host,
    // Training Area chỉ có 1 character tự điều khiển trực tiếp, không qua đồng bộ mạng.

    // ─────────────────────────────────────────────
    // Enable/disable controllers — thay cho switchCharacterMode (không có host/client)
    // ─────────────────────────────────────────────

    void EnableCharacterControllers(bool enable)
    {
        character.GetComponent<CharacterController>().enabled = enable;
        character.GetComponent<CharacterObjectService>().enabled = enable;
        character.GetComponent<CharacterMovementController>().enabled = enable;
        character.GetComponent<CharacterAttackController>().enabled = enable;
        character.GetComponent<CharacterDefenseController>().enabled = enable;
        character.GetComponent<CharacterDebuffController>().enabled = enable;
        character.GetComponent<CharacterTakeDamageController>().enabled = enable;
        character.GetComponent<CharacterActionController>().enabled = enable;
        character.GetComponent<CharacterObjectsData>().characterHurtBox.SetActive(enable);
    }

    // ─────────────────────────────────────────────
    // Customize / Accessories — chỉ 1 character, không cần index/list lookup
    // ─────────────────────────────────────────────

    public void ApplyCharacterAccessories(CharacterAccessoriesSet accessoriesSet)
    {
        CharacterAccessoriesController controller = character.GetComponent<CharacterAccessoriesController>();

        foreach (StoreItemsType type in Enum.GetValues(typeof(StoreItemsType)))
        {
            if (type == StoreItemsType.Skills) continue;

            CharacterAccessory accessory = accessoriesSet.TypeToAccessory(type);
            if (accessory == null) continue;

            if (string.IsNullOrEmpty(accessory.itemCode))
            {
                controller.TakeOff(StoreItemsTypeToAccessoriesPart(type));
            }
            else
            {
                AccessoriesListSO list = StoreData.instance.GetLocalListByType(type);
                if (list == null) continue;

                AccessoryItemSO itemSO = list.findByCode(accessory.itemCode);
                if (itemSO == null) continue;

                controller.PutOn(itemSO, accessory.properties);
            }
        }
    }

    private AccessoriesPart StoreItemsTypeToAccessoriesPart(StoreItemsType type)
    {
        return type switch
        {
            StoreItemsType.Hat => AccessoriesPart.Hat,
            StoreItemsType.Mask => AccessoriesPart.Mask,
            StoreItemsType.Neck => AccessoriesPart.Neck,
            StoreItemsType.Chest => AccessoriesPart.Chest,
            StoreItemsType.Back => AccessoriesPart.Back,
            StoreItemsType.Shoulders => AccessoriesPart.Shoulders,
            StoreItemsType.Gloves => AccessoriesPart.Gloves,
            StoreItemsType.Hip => AccessoriesPart.Hip,
            StoreItemsType.Leg => AccessoriesPart.Leg,
            StoreItemsType.Boots => AccessoriesPart.Boots,
            _ => AccessoriesPart.Hat
        };
    }

    void ApplyCharacterCustomize(CharacterCustomiziesSet characterCustomizies)
    {
        CharacterAccessoriesController controller = character.GetComponent<CharacterAccessoriesController>();

        if (characterCustomizies.races?.itemSO != null)
        {
            controller.SetCharacterCustomize(characterCustomizies.races.itemSO);
            controller.SetRacesColor(characterCustomizies.races.skinColor);
        }

        if (characterCustomizies.eyes?.itemSO != null)
        {
            controller.SetCharacterCustomize(characterCustomizies.eyes.itemSO);
            controller.SetEyesColors(characterCustomizies.eyes.irisColor, characterCustomizies.eyes.scleraColor, characterCustomizies.eyes.eyebrowEyelidColor);
        }

        if (characterCustomizies.mouth?.itemSO != null)
            controller.SetCharacterCustomize(characterCustomizies.mouth.itemSO);

        if (characterCustomizies.frontHair?.itemSO != null)
            controller.SetCharacterCustomize(characterCustomizies.frontHair.itemSO);

        if (characterCustomizies.topHair?.itemSO != null)
            controller.SetCharacterCustomize(characterCustomizies.topHair.itemSO);

        if (characterCustomizies.sideHair?.itemSO != null)
            controller.SetCharacterCustomize(characterCustomizies.sideHair.itemSO);

        if (characterCustomizies.frontHair?.itemSO != null)
            controller.SetHairColors(characterCustomizies.frontHair.primaryColor, characterCustomizies.frontHair.secondaryColor, characterCustomizies.frontHair.tertiaryColor);
    }

    void ApplyEmblem(CharacterAccessoriesController accessoriesController, Emblem emblem)
    {
        Material emblemMat = TransparentRenderCapture.instance.Capture(emblem);
        accessoriesController.SetEmblem(emblemMat);

        if (emblem.decals.Count > 0)
            TrainingHeaderUiController.instance.SetPlayerEmblem(emblemMat);
    }

    // ─────────────────────────────────────────────
    // Get Hit — giữ event + VFX, bỏ gửi packet qua HostPacketSender
    // ─────────────────────────────────────────────

    Action<int, bool> HandleCharacterGetHit()
    {
        return (damage, isDeflected) =>
        {
            PlayerCharacterGetHit(damage, isDeflected);
        };
    }

    public void PlayerCharacterGetHit(int damage, bool isDeflected)
    {
        Vector3 startPos = character.transform.position;

        character.GetComponent<CharacterStatesData>().currentEndurance =  character.GetComponent<CharacterStatsData>().endurance;

        if (!isDeflected)
        {
            HitVfxManager.instance.Show(startPos, true, damage);

            MatchHurtOverlayUiController.instance.PlayerGetHit();
        }
        else
        {
            HitVfxManager.instance.ShowDeflected(startPos, true);
        }
    }
}
