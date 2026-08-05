using System;
using UnityEngine;

namespace VoxelDestructionPro.Vox
{
    [Serializable]
    public class VoxVoxelData
    {
        public VoxVoxel[] voxels;
        public Color[] palette;
        public Vector3Int length;

        public int Volume => length.x * length.y * length.z;

        public VoxVoxelData(VoxVoxel[] voxels, Color[] palette, Vector3Int length)
        {
            this.voxels = voxels;
            this.palette = palette;
            this.length = length;
        }

        public bool InBounds(int x, int y, int z)
        {
            return x >= 0 && y >= 0 && z >= 0 && x < length.x && y < length.y && z < length.z;
        }

        public int ToIndex(int x, int y, int z)
        {
            return x + length.x * (y + length.y * z);
        }

        public void ToCoords(int index, out int x, out int y, out int z)
        {
            x = index % length.x;
            int yIndex = index / length.x;
            y = yIndex % length.y;
            z = yIndex / length.y;
        }

        public VoxVoxel GetVoxel(int x, int y, int z)
        {
            if (!InBounds(x, y, z))
                return default;
            return voxels[ToIndex(x, y, z)];
        }

        public void SetVoxel(int x, int y, int z, VoxVoxel voxel)
        {
            if (!InBounds(x, y, z))
                return;
            voxels[ToIndex(x, y, z)] = voxel;
        }

        public bool IsEmpty()
        {
            for (int i = 0; i < voxels.Length; i++)
            {
                if (voxels[i].active)
                    return false;
            }
            return true;
        }

        public static VoxVoxelData FromVox(
            int sizeX, int sizeY, int sizeZ,
            VoxVoxLoader.VoxVoxel[] xyzis,
            Color32[] palette256)
        {
            if (sizeX <= 0 || sizeY <= 0 || sizeZ <= 0)
                return null;

            Vector3Int size = new Vector3Int(sizeX, sizeY, sizeZ);

            Color32[] pal = palette256 != null && palette256.Length == 256
                ? palette256
                : VoxVoxLoader.GetDefaultPalette256();

            Color[] palette = new Color[256];
            for (int i = 0; i < 256; i++)
                palette[i] = new Color(pal[i].r / 255f, pal[i].g / 255f, pal[i].b / 255f, 1f);

            VoxVoxel[] voxels = new VoxVoxel[size.x * size.y * size.z];

            if (xyzis != null)
            {
                for (int i = 0; i < xyzis.Length; i++)
                {
                    int x = xyzis[i].x;
                    int y = xyzis[i].y;
                    int z = xyzis[i].z;

                    if (x < 0 || y < 0 || z < 0 || x >= size.x || y >= size.y || z >= size.z)
                        continue;

                    int paletteIndex = Mathf.Clamp(xyzis[i].colorIndex - 1, 0, 255);

                    voxels[x + size.x * (y + size.y * z)] = new VoxVoxel(true, paletteIndex);
                }
            }

            return new VoxVoxelData(voxels, palette, size);
        }
    }
}
