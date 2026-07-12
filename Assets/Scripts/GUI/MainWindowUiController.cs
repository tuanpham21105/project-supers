using System;
using UnityEngine;
using UnityEngine.UI;

public class MainWindowUiController : WindowUiController
{
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject cancelButton;
    [SerializeField] private GameObject findingMatchDisplayText;
    [SerializeField] private GameObject battleCostDisplayText;
    [SerializeField] private Button challengeButton;

    public override void OnCloseWindow()
    {
        base.OnCloseWindow();

        MainMenuCharacterModelController.instance.SetActiveRotateCharacter(false);
    }

    public override void OnOpenWindow()
    {
        base.OnOpenWindow();

        MainMenuCharacterModelController.instance.SetActiveRotateCharacter(true);
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
        battleCostDisplayText.SetActive(false);
        challengeButton.interactable = false;
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
        battleCostDisplayText.SetActive(true);
        challengeButton.interactable = true;
    }

    void handleStartMatchMakingSuccess()
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;

        playButton.SetActive(false);
        readyPanel.SetActive(false);
        cancelButton.SetActive(true);
        findingMatchDisplayText.SetActive(true);
        battleCostDisplayText.SetActive(false);
        challengeButton.interactable = false;
    }

    void handleStartMatchMakingFailed(string error)
    {
        MatchMakingController.instance.onStartMatchMakingSuccess -= handleStartMatchMakingSuccess;
        MatchMakingController.instance.onStartMatchMakingFailed -= handleStartMatchMakingFailed;
        
        playButton.SetActive(true);
        readyPanel.SetActive(false);
        cancelButton.SetActive(false);
        findingMatchDisplayText.SetActive(false);
        battleCostDisplayText.SetActive(true);
        challengeButton.interactable = true;
    }

    public void GoToTrainingArea()
    {
        MatchData.hostPlayer = PlayerData.instance.username;
        MatchData.players.Clear();
        MatchData.players.Add(PlayerData.instance.username);

        SceneService.instance.LoadScene("TrainingAreaScene");
    }
}
