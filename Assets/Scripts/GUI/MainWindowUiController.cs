using System;
using UnityEngine;

public class MainWindowUiController : WindowUiController
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameObject findingMatchDisplayText;

    void Start()
    {
        
    }

    void OnDestroy()
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;
    }

    public void PlayButton()
    {
        MatchMakingController.instance.onStartMatchMakingSuccess += handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed += handleStartMatchMakingFailed;
        
        MatchMakingController.instance.StartMatchMaking();

        playButton.SetActive(false);
        readyPanel.SetActive(true);
        cancelButton.SetActive(false);
        findingMatchDisplayText.SetActive(false);
    }

    public void CancelButton()
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;

        MatchMakingController.instance.CancelMatchMaking();
        playButton.SetActive(true);
        readyPanel.SetActive(false);
        cancelButton.SetActive(false);
        findingMatchDisplayText.SetActive(false);
    }

    void handleStartMatchMakingSuccess()
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;

        playButton.SetActive(false);
        readyPanel.SetActive(false);
        cancelButton.SetActive(true);
        findingMatchDisplayText.SetActive(true);
    }

    void handleStartMatchMakingFailed(string error)
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;
        
        playButton.SetActive(true);
        readyPanel.SetActive(false);
        cancelButton.SetActive(false);
        findingMatchDisplayText.SetActive(false);
    }
}
