using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChallengeRequestItemUiController : MonoBehaviour
{
    private string challengeId = "";

    [SerializeField] private TextMeshProUGUI usernameTextField;

    public void Initialize(string challengeId, string username)
    {
        this.challengeId = challengeId;
        usernameTextField.text = username;

        StartCoroutine(AutoReject());
    }

    IEnumerator AutoReject()
    {
        yield return new WaitForSeconds(10f);

        Response(false);
    }

    public void Response(bool state)
    {
        ChallengeController.instance.ResponseChallenge(challengeId, state);
        Destroy(gameObject);
    }
}
