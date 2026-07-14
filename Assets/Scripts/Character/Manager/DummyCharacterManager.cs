using System;
using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class DummyCharacterManager : MonoBehaviour
{
    public static DummyCharacterManager instance;

    [Header("Data")]
    [SerializeField] private GameObject dummy;
    private Action<int, bool> getHitHandler;
    private CharacterActions currentAction;

    // ─────────────────────────────────────────────
    // Events — vẫn giữ để UI/hệ thống khác lắng nghe
    // ─────────────────────────────────────────────

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
        getHitHandler = HandleDummyGetHit();
        dummy.GetComponent<CharacterTakeDamageController>().onGetHit += getHitHandler;

        EnableDummyControllers(true);
    }

    void FixedUpdate()
    {
        // Không cần build & gửi PlayersCharacterStatesDto — không có ai để đồng bộ tới
    }

    void OnDestroy()
    {
        instance = null;

        if (dummy != null)
        {
            dummy.GetComponent<CharacterTakeDamageController>().onGetHit -= getHitHandler;
        }
    }

    // ─────────────────────────────────────────────
    // Control — bỏ check MatchManager.IsMatchStart(), không có player param vì chỉ 1 character
    // ─────────────────────────────────────────────

    [ProButton]
    public void ControlDummyAction(string actionStr)
    {
        if (dummy == null) return;

        CharacterActionController controller = dummy.GetComponent<CharacterActionController>();

        if (!Enum.TryParse<CharacterActions>(actionStr, out CharacterActions action)) 
            return;

        IdleDummy();

        switch (action)
        {
            case CharacterActions.NormalAttack:
                controller.SetNormalAttack(true);
                break;
            case CharacterActions.Block:
                controller.SetBlock(true);
                break;
            case CharacterActions.Deflect:
                controller.SetDeflect(true);
                break;
        }

        currentAction = action;
    }

    public void IdleDummy()
    {
        if (dummy == null) return;

        CharacterActionController controller = dummy.GetComponent<CharacterActionController>();

        switch (currentAction)
        {
            case CharacterActions.NormalAttack:
                controller.SetNormalAttack(false);
                break;
            case CharacterActions.Block:
                controller.SetBlock(false);
                break;
            case CharacterActions.Deflect:
                controller.SetDeflect(false);
                break;
        }
    }

    public void ToggleKnockOutAttack(bool state)
    {
        CharacterStatsData statsData = dummy.GetComponent<CharacterStatsData>();

        statsData.normalAttackDamage = state ? 2600 : statsData.characterStatsSO.normalAttackDamage;
    }

    public void ControlDummyRotation(Vector3 direction)
    {
        if (dummy == null) return;

        CharacterActionController controller = dummy.GetComponent<CharacterActionController>();
        controller.SetRotation(direction);
    }

    // ControlCharacterAnimation và ControlCharacterStates KHÔNG cần —
    // đó là hàm đồng bộ character ở phía Client nhận dữ liệu từ Host,
    // Training Area chỉ có 1 character tự điều khiển trực tiếp, không qua đồng bộ mạng.

    // ─────────────────────────────────────────────
    // Enable/disable controllers — thay cho switchCharacterMode (không có host/client)
    // ─────────────────────────────────────────────

    void EnableDummyControllers(bool enable)
    {
        dummy.GetComponent<CharacterController>().enabled = enable;
        dummy.GetComponent<CharacterObjectService>().enabled = !enable;
        dummy.GetComponent<CharacterMovementController>().enabled = !enable;
        dummy.GetComponent<CharacterAttackController>().enabled = enable;
        dummy.GetComponent<CharacterDefenseController>().enabled = enable;
        dummy.GetComponent<CharacterDebuffController>().enabled = enable;
        dummy.GetComponent<CharacterTakeDamageController>().enabled = enable;
        dummy.GetComponent<CharacterActionController>().enabled = enable;
        dummy.GetComponent<CharacterObjectsData>().characterHurtBox.SetActive(enable);
    }

    // ─────────────────────────────────────────────
    // Get Hit — giữ event + VFX, bỏ gửi packet qua HostPacketSender
    // ─────────────────────────────────────────────

    Action<int, bool> HandleDummyGetHit()
    {
        return (damage, isDeflected) =>
        {
            DummyCharacterGetHit(damage, isDeflected);
        };
    }

    public void DummyCharacterGetHit(int damage, bool isDeflected)
    {
        Vector3 startPos = dummy.transform.position;

        dummy.GetComponent<CharacterStatesData>().currentEndurance =  dummy.GetComponent<CharacterStatsData>().endurance;

        if (!isDeflected)
        {
            HitVfxManager.instance.Show(startPos, false, damage);
        }
        else
        {
            HitVfxManager.instance.ShowDeflected(startPos, true);
        }
    }
}