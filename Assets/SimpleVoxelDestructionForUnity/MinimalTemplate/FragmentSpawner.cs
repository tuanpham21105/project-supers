using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    public class VoxFragmentSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject fragmentPrefab;
        [SerializeField] private Transform fragmentParent;

        [Header("Auto Parent Root")]
        [SerializeField] private string autoRootName = "FragmentsRoot";

        [Header("Fragment Destruction")]
        [SerializeField] private bool addDestructorToFragments = true;

        private Transform GetOrCreateRoot()
        {
            if (fragmentParent != null)
                return fragmentParent;

            GameObject existing = GameObject.Find(autoRootName);
            if (existing != null)
            {
                fragmentParent = existing.transform;
                return fragmentParent;
            }

            GameObject root = new GameObject(autoRootName);
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
            fragmentParent = root.transform;
            return fragmentParent;
        }

        public VoxVoxelObject SpawnFragment(VoxVoxelData data, Transform sourceTransform, Vector3 localOffset)
        {
            if (data == null || sourceTransform == null)
                return null;

            VoxVoxelObject sourceVoxelObj = sourceTransform.GetComponent<VoxVoxelObject>();
            VoxDestructor sourceDestructor = sourceTransform.GetComponent<VoxDestructor>();

            Transform parent = GetOrCreateRoot();

            GameObject instance;
            bool prefabIsSceneObject = fragmentPrefab != null && fragmentPrefab.scene.IsValid();

            if (fragmentPrefab != null && !prefabIsSceneObject)
            {
                instance = Instantiate(fragmentPrefab);
                instance.name = "VoxFragment";
            }
            else
            {
                instance = new GameObject("VoxFragment");
                instance.AddComponent<MeshFilter>();
                instance.AddComponent<MeshRenderer>();
            }

            instance.transform.SetParent(parent, worldPositionStays: true);

            instance.transform.position = sourceTransform.TransformPoint(localOffset);
            instance.transform.rotation = sourceTransform.rotation;
            instance.transform.localScale = sourceTransform.lossyScale;

            VoxVoxelObject voxelObject = instance.GetComponent<VoxVoxelObject>();
            if (voxelObject == null)
                voxelObject = instance.AddComponent<VoxVoxelObject>();

            voxelObject.CopySettingsFrom(sourceVoxelObj);
            voxelObject.AssignVoxelData(data);

            Rigidbody rb = instance.GetComponent<Rigidbody>();
            if (rb == null) rb = instance.AddComponent<Rigidbody>();
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            MeshCollider mc = instance.GetComponent<MeshCollider>();
            if (mc == null) mc = instance.AddComponent<MeshCollider>();
            mc.convex = true;

            MeshFilter mf = instance.GetComponent<MeshFilter>();
            if (mf != null) mc.sharedMesh = mf.sharedMesh;

            if (addDestructorToFragments)
            {
                VoxDestructor d = instance.GetComponent<VoxDestructor>();
                if (d == null) d = instance.AddComponent<VoxDestructor>();

                d.SetTarget(voxelObject);
                if (sourceDestructor != null)
                    d.CopyAudioFrom(sourceDestructor);
            }

            return voxelObject;
        }
    }
}
