using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class VoxVoxelObject : MonoBehaviour
    {
        [Header("Voxel Settings")]
        [SerializeField] private float voxelSize = 1f;

        [Header("Rendering")]
        [Tooltip("Material that uses vertex color (for example, Voxel/VertexColorUnlit).")]
        [SerializeField] private Material vertexColorMaterial;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        [SerializeField] private VoxVoxelData voxelData;

        public VoxVoxelData VoxelData => voxelData;
        public float VoxelSize => voxelSize;
        public Material VertexColorMaterial => vertexColorMaterial;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            EnsureMaterial();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            EnsureMaterial();

            if (!Application.isPlaying && voxelData != null)
                RebuildMesh();
        }
#endif

        private void EnsureMaterial()
        {
            if (meshRenderer == null)
                return;

            if (vertexColorMaterial != null)
                meshRenderer.sharedMaterial = vertexColorMaterial;
        }

        public void CopySettingsFrom(VoxVoxelObject source)
        {
            if (source == null)
                return;

            voxelSize = source.voxelSize;

            if (source.vertexColorMaterial != null)
                vertexColorMaterial = source.vertexColorMaterial;

            EnsureMaterial();
        }

        public void AssignVoxelData(VoxVoxelData data)
        {
            voxelData = data;
            RebuildMesh();
        }

        public void RebuildMesh()
        {
            if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();

            EnsureMaterial();

            Rigidbody rb = GetComponent<Rigidbody>();
            bool hadRB = rb != null;

            Vector3 savedPos = hadRB ? rb.position : transform.position;
            Quaternion savedRot = hadRB ? rb.rotation : transform.rotation;

            bool rbWasKinematic = false;
            bool rbHadCollisions = false;
            RigidbodyConstraints rbConstraints = RigidbodyConstraints.None;

            if (hadRB)
            {
                rbWasKinematic = rb.isKinematic;
                rbHadCollisions = rb.detectCollisions;
                rbConstraints = rb.constraints;

                rb.detectCollisions = false;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            MeshCollider mc = GetComponent<MeshCollider>();
            bool hadMC = mc != null;
            bool mcWasEnabled = false;
            bool mcWasTrigger = false;
            bool mcWasConvex = false;

            if (hadMC)
            {
                mcWasEnabled = mc.enabled;
                mcWasTrigger = mc.isTrigger;
                mcWasConvex = mc.convex;
                mc.enabled = false;
            }

            if (voxelData == null)
            {
                meshFilter.sharedMesh = null;

                if (hadMC)
                {
                    mc.sharedMesh = null;
                    mc.convex = mcWasConvex;
                    mc.isTrigger = mcWasTrigger;
                    mc.enabled = mcWasEnabled;
                }

                if (hadRB)
                {
                    rb.position = savedPos;
                    rb.rotation = savedRot;
                }
                else
                {
                    transform.SetPositionAndRotation(savedPos, savedRot);
                }

                Physics.SyncTransforms();

                if (hadRB)
                {
                    rb.constraints = rbConstraints;
                    rb.isKinematic = rbWasKinematic;
                    rb.detectCollisions = rbHadCollisions;

                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.Sleep();
                }

                return;
            }

            Mesh mesh = VoxMesher.BuildMesh(voxelData, voxelSize);
            meshFilter.sharedMesh = mesh;

            if (hadMC)
            {
                mc.sharedMesh = null;
                mc.sharedMesh = mesh;

                mc.convex = mcWasConvex;
                mc.isTrigger = mcWasTrigger;
                mc.enabled = mcWasEnabled;
            }

            if (hadRB)
            {
                rb.position = savedPos;
                rb.rotation = savedRot;
            }
            else
            {
                transform.SetPositionAndRotation(savedPos, savedRot);
            }

            Physics.SyncTransforms();

            if (hadRB)
            {
                rb.constraints = rbConstraints;
                rb.isKinematic = rbWasKinematic;
                rb.detectCollisions = rbHadCollisions;

                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
        }
    }
}
