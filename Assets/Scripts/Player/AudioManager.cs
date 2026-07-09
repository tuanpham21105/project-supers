using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayerData.instance.onMasterVolumeChange += handleMasterVolumeChange;
    }

    void OnDestroy()
    {
        PlayerData.instance.onMasterVolumeChange -= handleMasterVolumeChange;
    }

    void handleMasterVolumeChange(int value)
    {
        AudioListener.volume = (float) value / 100.0f;        
    }
}
