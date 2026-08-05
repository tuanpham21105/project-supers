using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelDestructionPro.Vox;

public class CharacterHitBoxService : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    [SerializeField] private CharacterHitBoxesEvents characterHitBoxesEvents;

    // [Constant]
    [Header("Constant")]
    [SerializeField] private AttackTypes attackType;

    // [Voxel Destruction]
    [Header("Voxel Destruction")]
    [SerializeField] private LayerMask voxelLayerMask = ~0;
    [SerializeField] private bool isolateFragments = true;

    private Collider cachedCollider;
    private SphereCollider cachedSphereCollider;
    private Collider[] overlapBuffer = new Collider[32];

    void Start()
    {
        cachedCollider = GetComponent<Collider>();
        cachedSphereCollider = cachedCollider as SphereCollider;

        characterHitBoxesEvents.OnAttackInterrupt += HandleAttackInterrupt;
        if (attackType == AttackTypes.fly_attack)
        {
            characterHitBoxesEvents.OnStartFlyAttack += HandleStartFlyAttack;
            characterHitBoxesEvents.OnEndFlyAttack += HandleEndFlyAttack;
        }
    }

    void OnDestroy()
    {
        characterHitBoxesEvents.OnAttackInterrupt -= HandleAttackInterrupt;
    }

    void FixedUpdate()
    {
        if (cachedSphereCollider == null || !cachedSphereCollider.enabled)
            return;

        Vector3 worldCenter = cachedSphereCollider.transform.TransformPoint(cachedSphereCollider.center);

        Vector3 scale = cachedSphereCollider.transform.lossyScale;
        float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        float worldRadius = cachedSphereCollider.radius * Mathf.Max(maxScale, 0.0001f);

        int count = Physics.OverlapSphereNonAlloc(worldCenter, worldRadius, overlapBuffer, voxelLayerMask);

        for (int i = 0; i < count; i++)
        {
            Collider hit = overlapBuffer[i];
            if (hit == null)
                continue;

            VoxVoxelObject voxelObject = hit.GetComponentInParent<VoxVoxelObject>();
            if (voxelObject == null)
                continue;

            VoxDestructor destructor = voxelObject.GetComponent<VoxDestructor>();
            if (destructor == null)
                continue;

            bool destroyed = destructor.DestroySphere(worldCenter, worldRadius);

            if (destroyed && isolateFragments)
            {
                VoxIsolation isolation = voxelObject.GetComponent<VoxIsolation>();
                if (isolation != null)
                    isolation.Isolate();
            }
        }
    }

    private void HandleAttackInterrupt()
    {
        if (cachedCollider != null)
            cachedCollider.enabled = false;
    }

    private void HandleStartFlyAttack()
    {
        if (cachedCollider != null)
            cachedCollider.enabled = true;
    }

    private void HandleEndFlyAttack()
    {
        if (cachedCollider != null)
            cachedCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (characterHitBoxesEvents == null) return;

        CharacterHurtBoxService hurtBox = other.GetComponent<CharacterHurtBoxService>();
        if (hurtBox == null)
            return;

        characterHitBoxesEvents.EmitAttackHit(hurtBox.GetCharacter(), attackType);
    }
}
