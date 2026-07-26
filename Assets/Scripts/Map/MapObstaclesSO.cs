using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapObstaclesSO", menuName = "Game/Map Obstacles")]
public class MapObstaclesSO : ScriptableObject
{
    public List<MapObstacleSpace> obstacleSpaces = new List<MapObstacleSpace>();
}
