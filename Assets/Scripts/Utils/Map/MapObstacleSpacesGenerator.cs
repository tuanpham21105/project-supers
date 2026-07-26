using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapObstacleGenerator : EditorWindow
{
    private Vector2Int minXZ = new Vector2Int(-150, -150);
    private Vector2Int maxXZ = new Vector2Int(150, 150);

    private int height = 0;

    private Vector3Int obstacleSize = Vector3Int.one * 30;

    private int minSpacing = 50;
    private int maxSpacing = 80;

    [MenuItem("Tools/Generate Map Obstacles")]
    static void Open()
    {
        GetWindow<MapObstacleGenerator>("Obstacle Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Range", EditorStyles.boldLabel);

        minXZ = EditorGUILayout.Vector2IntField("Min XZ", minXZ);
        maxXZ = EditorGUILayout.Vector2IntField("Max XZ", maxXZ);

        height = EditorGUILayout.IntField("Height", height);

        obstacleSize = EditorGUILayout.Vector3IntField("Obstacle Size", obstacleSize);

        minSpacing = EditorGUILayout.IntField("Min Spacing", minSpacing);
        maxSpacing = EditorGUILayout.IntField("Max Spacing", maxSpacing);

        if (GUILayout.Button("Generate"))
        {
            Generate();
        }
    }

    private void Generate()
    {
        MapObstaclesSO asset = ScriptableObject.CreateInstance<MapObstaclesSO>();

        List<MapObstacleSpace> spaces = new List<MapObstacleSpace>();

        int z = minXZ.y;

        while (z <= maxXZ.y)
        {
            int x = minXZ.x;

            while (x <= maxXZ.x)
            {
                spaces.Add(new MapObstacleSpace()
                {
                    start = new Vector3Int(x, height, z),
                    size = obstacleSize
                });

                x += obstacleSize.x + Random.Range(minSpacing, maxSpacing + 1);
            }

            z += obstacleSize.z + Random.Range(minSpacing, maxSpacing + 1);
        }

        asset.obstacleSpaces = spaces;

        string path = EditorUtility.SaveFilePanelInProject(
            "Save MapObstaclesSO",
            "MapObstacles",
            "asset",
            "Choose save location");

        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(asset);
        }
    }
}