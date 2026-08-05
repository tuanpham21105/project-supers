using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace VoxelDestructionPro.Vox
{
    [ExecuteAlways]
    public class VoxVoxLoader : MonoBehaviour
    {
        [Header("Vox Source (Project Assets)")]
        [Tooltip("Path to .vox relative to the project root. Examples:\n\"Assets/Models/demo/castle.vox\"\n\"Models/demo/castle\" (\"Assets/\" and \".vox\" will be added automatically).")]
        [SerializeField] private string assetPathInProject;

        [Header("Target")]
        [SerializeField] private VoxVoxelObject targetObject;

        [ContextMenu("Load VOX (Editor / Play)")]
        public void LoadInEditor()
        {
            LoadAndApply(true);
        }

        public bool LoadAndApply(bool log = false)
        {
            if (targetObject == null)
                targetObject = GetComponent<VoxVoxelObject>();

            if (targetObject == null)
            {
                if (log) Debug.LogWarning("VoxVoxLoader: Missing VoxVoxelObject target.", this);
                return false;
            }

            VoxVoxelData data = LoadVoxelData(log);
            if (data == null)
                return false;

            targetObject.AssignVoxelData(data);

            if (log)
                Debug.Log($"VoxVoxLoader: Loaded '{assetPathInProject}' into '{targetObject.name}'.", this);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(targetObject);
                EditorSceneManager.MarkSceneDirty(gameObject.scene);
            }
#endif
            return true;
        }

        public VoxVoxelData LoadVoxelData(bool log = false)
        {
            if (string.IsNullOrWhiteSpace(assetPathInProject))
            {
                if (log) Debug.LogWarning("VoxVoxLoader: Asset path is empty.", this);
                return null;
            }

            string fullPath = GetFullProjectPath(assetPathInProject);

            if (!File.Exists(fullPath))
            {
                if (log) Debug.LogWarning($"VoxVoxLoader: Vox file not found at '{fullPath}'.", this);
                return null;
            }

            try
            {
                VoxModel model = ReadVoxFirstModel(fullPath);
                if (model.sizeX <= 0 || model.sizeY <= 0 || model.sizeZ <= 0 || model.voxels == null)
                {
                    if (log) Debug.LogWarning("VoxVoxLoader: Failed to read VOX model.", this);
                    return null;
                }

                return VoxVoxelData.FromVox(model.sizeX, model.sizeY, model.sizeZ, model.voxels, model.palette256);
            }
            catch (Exception e)
            {
                if (log) Debug.LogError($"VoxVoxLoader: Exception while reading .vox: {e}", this);
                return null;
            }
        }

        [Serializable]
        public struct VoxVoxel
        {
            public byte x, y, z;
            public byte colorIndex;
        }

        private struct VoxModel
        {
            public int sizeX, sizeY, sizeZ;
            public VoxVoxel[] voxels;
            public Color32[] palette256;
        }

        private static VoxModel ReadVoxFirstModel(string path)
        {
            VoxModel model = new VoxModel
            {
                palette256 = null
            };

            using (var br = new BinaryReader(File.OpenRead(path)))
            {
                string magic = new string(br.ReadChars(4));
                if (magic != "VOX ")
                    throw new Exception("Not a VOX file (missing 'VOX ').");

                int version = br.ReadInt32();

                string mainId = new string(br.ReadChars(4));
                if (mainId != "MAIN")
                    throw new Exception("VOX missing MAIN chunk.");

                int mainContent = br.ReadInt32();
                int mainChildren = br.ReadInt32();
                if (mainContent > 0) br.ReadBytes(mainContent);

                long mainEnd = br.BaseStream.Position + mainChildren;

                bool gotSize = false;
                bool gotXYZI = false;

                while (br.BaseStream.Position < mainEnd)
                {
                    string id = new string(br.ReadChars(4));
                    int contentSize = br.ReadInt32();
                    int childrenSize = br.ReadInt32();

                    long contentStart = br.BaseStream.Position;

                    if (id == "SIZE")
                    {
                        model.sizeX = br.ReadInt32();
                        model.sizeY = br.ReadInt32();
                        model.sizeZ = br.ReadInt32();
                        gotSize = true;
                    }
                    else if (id == "XYZI")
                    {
                        int n = br.ReadInt32();
                        model.voxels = new VoxVoxel[n];
                        for (int i = 0; i < n; i++)
                        {
                            model.voxels[i].x = br.ReadByte();
                            model.voxels[i].y = br.ReadByte();
                            model.voxels[i].z = br.ReadByte();
                            model.voxels[i].colorIndex = br.ReadByte();
                        }
                        gotXYZI = true;
                    }
                    else if (id == "RGBA")
                    {
                        Color32[] pal = new Color32[256];
                        for (int i = 0; i < 256; i++)
                        {
                            byte r = br.ReadByte();
                            byte g = br.ReadByte();
                            byte b = br.ReadByte();
                            byte a = br.ReadByte();
                            pal[i] = new Color32(r, g, b, 255);
                        }
                        model.palette256 = pal;
                    }

                    long contentRead = br.BaseStream.Position - contentStart;
                    long toSkip = contentSize - contentRead;
                    if (toSkip > 0) br.ReadBytes((int)toSkip);

                    if (childrenSize > 0) br.ReadBytes(childrenSize);
                }

                if (!gotSize || !gotXYZI)
                    throw new Exception("VOX missing SIZE or XYZI chunk.");
            }

            return model;
        }

        public static Color32[] GetDefaultPalette256()
        {
            Color32[] p = new Color32[256];
            p[0] = new Color32(0, 0, 0, 0);
            for (int i = 1; i < 256; i++)
            {
                byte v = (byte)i;
                p[i] = new Color32(v, v, v, 255);
            }
            return p;
        }

        private static string GetFullProjectPath(string pathInProject)
        {
            string p = pathInProject.Replace('\\', '/').Trim();

            if (!p.EndsWith(".vox", StringComparison.OrdinalIgnoreCase))
                p += ".vox";

            if (!p.StartsWith("Assets/"))
                p = "Assets/" + p.TrimStart('/');

            string afterAssets = p.Substring("Assets/".Length);
            string fullPath = Path.Combine(Application.dataPath, afterAssets);
            return fullPath;
        }
    }
}
