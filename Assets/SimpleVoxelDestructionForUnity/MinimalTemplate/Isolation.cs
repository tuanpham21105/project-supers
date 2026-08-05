using System.Collections.Generic;
using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    public class VoxIsolation : MonoBehaviour
    {
        [SerializeField] private VoxVoxelObject targetObject;
        [SerializeField] private VoxFragmentSpawner fragmentSpawner;

        [Header("Fragment Settings")]
        [Tooltip("Minimum number of voxels in a cluster to spawn a separate fragment.")]
        [Min(1)]
        [SerializeField] private int minClusterSize = 20;

        [Tooltip("Maximum number of fragments per Isolate call. Remaining clusters stay in the base object.")]
        [Min(1)]
        [SerializeField] private int maxFragmentsPerCall = 4;

        public void Isolate()
        {
            if (targetObject == null)
                targetObject = GetComponent<VoxVoxelObject>();

            if (fragmentSpawner == null)
                fragmentSpawner = GetComponent<VoxFragmentSpawner>();

            if (targetObject == null || fragmentSpawner == null)
                return;

            VoxVoxelData data = targetObject.VoxelData;
            if (data == null || data.IsEmpty())
                return;

            bool[] visited = new bool[data.Volume];
            List<List<int>> clusters = new List<List<int>>();
            List<int> clusterSizes = new List<int>();

            int maxClusterIndex = -1;
            int maxClusterSize = 0;

            for (int i = 0; i < data.voxels.Length; i++)
            {
                if (visited[i] || !data.voxels[i].active)
                    continue;

                List<int> cluster = FloodFill(data, i, visited);
                clusters.Add(cluster);
                clusterSizes.Add(cluster.Count);

                if (cluster.Count > maxClusterSize)
                {
                    maxClusterSize = cluster.Count;
                    maxClusterIndex = clusters.Count - 1;
                }
            }

            if (clusters.Count <= 1)
                return;

            float voxelSize = targetObject != null ? targetObject.VoxelSize : 1f;

            List<int> candidateIndices = new List<int>();
            for (int i = 0; i < clusters.Count; i++)
            {
                if (i == maxClusterIndex)
                    continue;

                if (clusterSizes[i] < minClusterSize)
                    continue;

                candidateIndices.Add(i);
            }

            if (candidateIndices.Count == 0)
                return;

            candidateIndices.Sort((a, b) => clusterSizes[b].CompareTo(clusterSizes[a]));

            int spawnedCount = 0;

            foreach (int idx in candidateIndices)
            {
                if (spawnedCount >= maxFragmentsPerCall)
                    break;

                List<int> cluster = clusters[idx];
                VoxVoxelData fragmentData = ExtractCluster(data, cluster, out Vector3 voxelOffset);
                if (fragmentData == null)
                    continue;

                Vector3 localOffsetUnits = voxelOffset * voxelSize;

                fragmentSpawner.SpawnFragment(fragmentData, targetObject.transform, localOffsetUnits);
                spawnedCount++;
            }

            targetObject.RebuildMesh();
        }

        private List<int> FloodFill(VoxVoxelData data, int startIndex, bool[] visited)
        {
            List<int> cluster = new List<int>();
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(startIndex);
            visited[startIndex] = true;

            while (queue.Count > 0)
            {
                int index = queue.Dequeue();
                cluster.Add(index);

                data.ToCoords(index, out int x, out int y, out int z);
                EnqueueIfActive(data, visited, queue, x + 1, y, z);
                EnqueueIfActive(data, visited, queue, x - 1, y, z);
                EnqueueIfActive(data, visited, queue, x, y + 1, z);
                EnqueueIfActive(data, visited, queue, x, y - 1, z);
                EnqueueIfActive(data, visited, queue, x, y, z + 1);
                EnqueueIfActive(data, visited, queue, x, y, z - 1);
            }

            return cluster;
        }

        private void EnqueueIfActive(VoxVoxelData data, bool[] visited, Queue<int> queue, int x, int y, int z)
        {
            if (!data.InBounds(x, y, z))
                return;

            int index = data.ToIndex(x, y, z);
            if (visited[index])
                return;

            if (!data.voxels[index].active)
                return;

            visited[index] = true;
            queue.Enqueue(index);
        }

        private VoxVoxelData ExtractCluster(VoxVoxelData source, List<int> cluster, out Vector3 localOffsetInVoxels)
        {
            localOffsetInVoxels = Vector3.zero;

            if (cluster == null || cluster.Count == 0)
                return null;

            Vector3Int min = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int max = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

            for (int i = 0; i < cluster.Count; i++)
            {
                source.ToCoords(cluster[i], out int x, out int y, out int z);
                min = Vector3Int.Min(min, new Vector3Int(x, y, z));
                max = Vector3Int.Max(max, new Vector3Int(x, y, z));
            }

            Vector3Int size = max - min + Vector3Int.one;
            VoxVoxel[] voxels = new VoxVoxel[size.x * size.y * size.z];

            for (int i = 0; i < cluster.Count; i++)
            {
                source.ToCoords(cluster[i], out int x, out int y, out int z);
                VoxVoxel voxel = source.GetVoxel(x, y, z);
                voxel.active = true;

                int localX = x - min.x;
                int localY = y - min.y;
                int localZ = z - min.z;
                voxels[localX + size.x * (localY + size.y * localZ)] = voxel;

                VoxVoxel empty = voxel;
                empty.active = false;
                source.SetVoxel(x, y, z, empty);
            }

            localOffsetInVoxels = new Vector3(min.x, min.y, min.z);
            return new VoxVoxelData(voxels, source.palette, size);
        }
    }
}
