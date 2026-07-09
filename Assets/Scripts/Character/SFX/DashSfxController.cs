using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSfxController : MonoBehaviour
{
    [SerializeField] private CharacterStatesData characterStatesData;

    void Start()
    {
        characterStatesData.onDashFlagChange += handleDashFlagChange;
    }

    void OnDestroy()
    {
        characterStatesData.onDashFlagChange -= handleDashFlagChange;
    }

    void handleDashFlagChange(bool value)
    {
        if (value) 
            GetComponent<AudioSource>().Play();
    }
}
