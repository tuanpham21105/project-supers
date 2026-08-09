using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    public class VoxDestructor : MonoBehaviour
    {
        [SerializeField] private VoxVoxelObject targetObject;

        [Header("Audio")]
        [SerializeField] private AudioClip destroyClip;
        [Range(0f, 1f)]
        [SerializeField] private float destroyVolume = 1f;

        [Tooltip("If true, plays sound at the hit world position via a temporary AudioSource.")]
        [SerializeField] private bool playSoundAtHitPoint = true;

        [Range(0f, 1f)]
        [SerializeField] private float tempAudioSpatialBlend = 1f;
        [SerializeField] private float tempAudioMinDistance = 1f;
        [SerializeField] private float tempAudioMaxDistance = 25f;

        private AudioSource audioSource;

        private void Awake()
        {
            if (targetObject == null)
                targetObject = GetComponent<VoxVoxelObject>();

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
            }
        }

        public void SetTarget(VoxVoxelObject obj)
        {
            targetObject = obj;
        }

        public void CopyAudioFrom(VoxDestructor other)
        {
            if (other == null) return;
            destroyClip = other.destroyClip;
            destroyVolume = other.destroyVolume;
            playSoundAtHitPoint = other.playSoundAtHitPoint;
            tempAudioSpatialBlend = other.tempAudioSpatialBlend;
            tempAudioMinDistance = other.tempAudioMinDistance;
            tempAudioMaxDistance = other.tempAudioMaxDistance;
        }

        public bool DestroySphere(Vector3 worldPosition, float radius)
        {
            if (targetObject == null || targetObject.VoxelData == null)
                return false;

            VoxVoxelData data = targetObject.VoxelData;

            Vector3 localPoint = targetObject.transform.InverseTransformPoint(worldPosition);

            float voxelSize = Mathf.Max(targetObject.VoxelSize, 0.0001f);
            Vector3 centerVox = localPoint / voxelSize;

            Vector3 ls = targetObject.transform.lossyScale;
            float scaleMax = Mathf.Max(Mathf.Abs(ls.x), Mathf.Abs(ls.y), Mathf.Abs(ls.z));
            scaleMax = Mathf.Max(scaleMax, 0.0001f);

            float radiusVox = radius / (voxelSize * scaleMax);
            float radiusVoxSq = radiusVox * radiusVox;

            Vector3Int min = new Vector3Int(
                Mathf.FloorToInt(centerVox.x - radiusVox - 0.5f),
                Mathf.FloorToInt(centerVox.y - radiusVox - 0.5f),
                Mathf.FloorToInt(centerVox.z - radiusVox - 0.5f)
            );
            Vector3Int max = new Vector3Int(
                Mathf.CeilToInt(centerVox.x + radiusVox + 0.5f),
                Mathf.CeilToInt(centerVox.y + radiusVox + 0.5f),
                Mathf.CeilToInt(centerVox.z + radiusVox + 0.5f)
            );

            bool anyDestroyed = false;

            for (int z = min.z; z <= max.z; z++)
                for (int y = min.y; y <= max.y; y++)
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!data.InBounds(x, y, z))
                            continue;

                        Vector3 voxelCenter = new Vector3(x, y, z);
                        if ((voxelCenter - centerVox).sqrMagnitude > radiusVoxSq)
                            continue;

                        VoxVoxel voxel = data.GetVoxel(x, y, z);
                        if (!voxel.active)
                            continue;

                        voxel.active = false;
                        data.SetVoxel(x, y, z, voxel);
                        anyDestroyed = true;
                    }

            if (!anyDestroyed)
                return false;

            targetObject.RebuildMesh();

            if (destroyClip != null)
            {
                if (playSoundAtHitPoint)
                    PlayClipAtPointSafe(destroyClip, worldPosition, destroyVolume);
                else
                    audioSource.PlayOneShot(destroyClip, destroyVolume);
            }

            return true;
        }

        private void PlayClipAtPointSafe(AudioClip clip, Vector3 position, float volume)
        {
            GameObject go = new GameObject("OneShotAudio_Destroy");
            go.transform.position = position;

            AudioSource a = go.AddComponent<AudioSource>();
            a.playOnAwake = false;
            a.clip = clip;
            a.volume = volume;

            a.spatialBlend = tempAudioSpatialBlend;
            a.minDistance = tempAudioMinDistance;
            a.maxDistance = tempAudioMaxDistance;

            a.Play();
            Destroy(go, 3f);
        }
    }
}
