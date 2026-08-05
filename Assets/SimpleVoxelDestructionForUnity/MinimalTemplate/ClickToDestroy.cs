using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    public class VoxClickToDestroy : MonoBehaviour
    {
        [Header("Input / Camera")]
        [Tooltip("Camera used for raycasting. If null, Camera.main will be used.")]
        public Camera cam;

        [Tooltip("If enabled, holding mouse means continuous fire, Shift + click means a single shot.")]
        public bool useShiftForSingleClick = true;

        [Header("Destruction")]
        [Tooltip("Destruction radius in world units.")]
        public float destructionRadius = 2f;

        [Tooltip("If true, calls VoxIsolation after destruction to separate loose fragments.")]
        public bool autoIsolateFragments = true;

        private void Awake()
        {
            if (cam == null)
                cam = Camera.main;
        }

        private void Update()
        {
            if (cam == null)
                return;

            bool oneClickMode = useShiftForSingleClick && Input.GetKey(KeyCode.LeftShift);

            bool fireNow =
                (!oneClickMode && Input.GetMouseButton(0)) ||
                (oneClickMode && Input.GetMouseButtonDown(0));

            if (!fireNow)
                return;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 999f))
                return;

            VoxVoxelObject voxelObject = hit.transform.GetComponentInParent<VoxVoxelObject>();
            if (voxelObject == null)
                return;

            VoxDestructor destructor = voxelObject.GetComponent<VoxDestructor>();
            if (destructor == null)
                return;

            destructor.DestroySphere(hit.point, destructionRadius);

            if (autoIsolateFragments)
            {
                VoxIsolation isolation = voxelObject.GetComponent<VoxIsolation>();
                if (isolation != null)
                {
                    isolation.Isolate();
                }
            }
        }
    }
}
