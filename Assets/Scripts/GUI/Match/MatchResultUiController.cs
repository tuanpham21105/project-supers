using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchResultUiController : MonoBehaviour
{
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private GameObject loseOverlay;
    [SerializeField] private GameObject tieOverlay;

    void Start()
    {
        MatchFinishManager.instance.onMatchFinish += handleMatchFinish;
    }

    void OnDestroy()
    {
        if (MatchFinishManager.instance != null)
            MatchFinishManager.instance.onMatchFinish -= handleMatchFinish;   
    }

    void handleMatchFinish(int status)
    {
        switch (status)
        {
            case 0:
                tieOverlay.SetActive(true);
                return;
            case 1:
                winOverlay.SetActive(true);
                return;
            default:
                loseOverlay.SetActive(true);
                return;
        }
    }
}
