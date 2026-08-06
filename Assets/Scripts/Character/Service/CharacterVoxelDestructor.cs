using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelDestructionPro.Vox;

public class CharacterVoxelDestructor : MonoBehaviour
{
    private CharacterStatesData characterStatesData;

    // [Voxel Destruction]
    [Header("Voxel Destruction")]
    [SerializeField] private float voxelDestroySpeedThreshold = 1f;
    [SerializeField] private float voxelDestroyRadius = 1f;
    [SerializeField] private LayerMask voxelLayerMask = ~0;
    [SerializeField] private bool isolateVoxelFragments = true;

    private Collider[] voxelOverlapBuffer = new Collider[16];

    void Start()
    {
        characterStatesData = GetComponent<CharacterStatesData>();
    }

    void FixedUpdate()
    {
        DestroyVoxelsOnHighSpeed();
    }
    private const string NormalLayerName = "Character";
    private const string FastMoveLayerName = "Character Fast Move";

    private void DestroyVoxelsOnHighSpeed()
    {
        bool isFastMoving = characterStatesData.currentPow2AllSpeed > voxelDestroySpeedThreshold;
        string targetLayerName = isFastMoving ? FastMoveLayerName : NormalLayerName;

        if (gameObject.layer != LayerMask.NameToLayer(targetLayerName))
            gameObject.layer = LayerMask.NameToLayer(targetLayerName);

        if (!isFastMoving)
            return;

        Vector3 center = transform.position;

        int count = Physics.OverlapSphereNonAlloc(center, voxelDestroyRadius, voxelOverlapBuffer, voxelLayerMask);

        for (int i = 0; i < count; i++)
        {
            Collider hit = voxelOverlapBuffer[i];
            if (hit == null)
                continue;

            VoxVoxelObject voxelObject = hit.GetComponentInParent<VoxVoxelObject>();
            if (voxelObject == null)
                continue;

            VoxDestructor destructor = voxelObject.GetComponent<VoxDestructor>();
            if (destructor == null)
                continue;

            bool destroyed = destructor.DestroySphere(center, voxelDestroyRadius);

            if (destroyed && isolateVoxelFragments)
            {
                VoxIsolation isolation = voxelObject.GetComponent<VoxIsolation>();
                if (isolation != null)
                    isolation.Isolate();
            }
        }
    }
}
