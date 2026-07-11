using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultSceneUiController : MonoBehaviour
{
    [SerializeField] private GameObject winLabel;
    [SerializeField] private GameObject loseLabel;
    [SerializeField] private GameObject tieLabel;
    [SerializeField] private GameObject proccessingLabel;
    [SerializeField] private TextMeshProUGUI usernameTextField;
    [SerializeField] private TextMeshProUGUI levelsTextField;
    [SerializeField] private TextMeshProUGUI expTextField;
    [SerializeField] private Image expProgressBar;
    [SerializeField] private TextMeshProUGUI pointsTextField;
    [SerializeField] private TextMeshProUGUI diffTextField;
    [SerializeField] private TextMeshProUGUI remainSecondsTextField;
    [SerializeField] private Image logoImage;
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;
    [SerializeField] private Sprite tieSprite;
    [SerializeField] private Sprite proccessingSprite;
    [SerializeField] private int autoReturnTime = 10;
    private Coroutine autoReturnCoroutine;

    void Start()
    {
        usernameTextField.text = PlayerData.instance.username;

        autoReturnCoroutine = StartCoroutine(AutoReturnToMainMenu());

        PlayerMatchService.instance.GetMatchResultById(
            MatchData.matchId,
            (response) =>
            {
                if (response.isFinish)
                {
                    proccessingLabel.SetActive(false);

                    if (response.winnerUsername.Equals(""))
                    {
                        tieLabel.SetActive(true);
                        logoImage.sprite = tieSprite;
                    }
                    else if (response.winnerUsername.Equals(PlayerData.instance.username))
                    {
                        winLabel.SetActive(true);
                        logoImage.sprite = winSprite;
                    }
                    else
                    {
                        loseLabel.SetActive(true);
                        logoImage.sprite = loseSprite;
                    }
                }
            },
            (code, error) =>
            {
                Debug.LogError($"[MatchResultSceneController] Failed to load match result: {error}");
            } 
        );

        PlayerInventoryService.instance.GetPlayerInventory(
            (response) =>
            {
                levelsTextField.text = "Lvls." + response.levels;
                expProgressBar.fillAmount = BigNumberStringify.ratio(response.exp, response.levelsUpExp);
                expTextField.text = BigNumberStringify.decorate(response.exp) + " EXP / " + BigNumberStringify.decorate(response.levelsUpExp) + " EXP";
                pointsTextField.text = "Heroes Points: " + BigNumberStringify.decorate(response.points);
                long diff = response.points - PlayerData.instance.Points;
                if (diff == 0)
                {
                    diffTextField.gameObject.SetActive(false);
                }
                else if (diff > 0)
                {
                    diffTextField.text = "+" + diff;
                    diffTextField.color = Color.green;
                }
                else
                {
                    diffTextField.text = "" + diff;  
                    diffTextField.color = Color.red; 
                }

                PlayerData.instance.Points = response.points;
                PlayerData.instance.levels = response.levels;
                PlayerData.instance.exp = response.exp;
                PlayerData.instance.levelsUpExp = response.levelsUpExp;
            },
            (code, error) =>
            {
                Debug.LogError($"[MatchResultSceneController] Failed to fetch player data: {error}");
            }
        );
    }

    public void ReturnToMainMenu()
    {
        if (autoReturnCoroutine != null)
        {
            StopCoroutine(autoReturnCoroutine);
        }

        SceneService.instance.LoadScene("StartScene");
    }

    IEnumerator AutoReturnToMainMenu()
    {
        for (int i = autoReturnTime; i >= 0; i--)
        {
            remainSecondsTextField.text = i + "s";

            yield return new WaitForSeconds(1f);
        }

        autoReturnCoroutine = null;

        ReturnToMainMenu();
    }
}
