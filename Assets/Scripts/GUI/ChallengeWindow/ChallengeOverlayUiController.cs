using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeOverlayUiController : MonoBehaviour
{
    [SerializeField] private Transform content;

    [SerializeField] private GameObject itemPrefab;
    
    void Start()
    {
        ChallengeController.instance.onChallengeCome += handleChallengeCome;
    }

    void OnDestroy()
    {
        ChallengeController.instance.onChallengeCome -= handleChallengeCome;
    }

    void handleChallengeCome(string id, string username)
    {

        GameObject newItem = Instantiate(itemPrefab, content);

        newItem.GetComponent<ChallengeRequestItemUiController>().Initialize(id, username);
    }
}
