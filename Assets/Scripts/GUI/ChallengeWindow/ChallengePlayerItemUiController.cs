using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengePlayerItemUiController : MonoBehaviour
{
    private static int cooldownTime = 10;

    private string username;
    [SerializeField] private TextMeshProUGUI usernameTextField;
    [SerializeField] private Button challengeButton;

    public void Initialize(string username)
    {
        this.username = username;
        usernameTextField.text = username;
    }

    public void Challenge()
    {
        challengeButton.interactable = false;

        ChallengeController.instance.SendChallenge(username);

        StartCoroutine(ChallengeCooldown());
    }

    IEnumerator ChallengeCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);

        challengeButton.interactable = true;
    }
}
