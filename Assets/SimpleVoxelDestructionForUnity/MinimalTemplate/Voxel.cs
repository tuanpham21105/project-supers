using System;

namespace VoxelDestructionPro.Vox
{
    [Serializable]
    public struct VoxVoxel : IEquatable<VoxVoxel>
    {
        public bool active;
        public int colorIndex;
        public int normal;

        public VoxVoxel(bool active, int colorIndex)
        {
            this.active = active;
            this.colorIndex = colorIndex;
            normal = 0;
        }

        public bool Equals(VoxVoxel other)
        {
            return active == other.active && colorIndex == other.colorIndex && normal == other.normal;
        }

        public override bool Equals(object obj)
        {
            return obj is VoxVoxel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + active.GetHashCode();
                hash = hash * 23 + colorIndex.GetHashCode();
                hash = hash * 23 + normal.GetHashCode();
                return hash;
            }
        }
    }
}
