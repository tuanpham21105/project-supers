using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour
{
    public static MapData instance;

    [SerializeField] private List<Transform> spawnPoints;
    public List<Transform> getSpawnPoints() => spawnPoints;

    void Awake()
    {
        instance = this;
    }
}
