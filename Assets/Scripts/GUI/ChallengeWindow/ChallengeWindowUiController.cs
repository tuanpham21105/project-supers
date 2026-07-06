using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeWindowUiController : WindowUiController
{
    [Header("Configs")]
    [SerializeField] private int maxItemNumbers = 20;

    [Header("Resources")]
    [SerializeField] private GameObject playerItemPrefab;

    [Header("Search bar objects")]
    [SerializeField] private TMP_InputField searchBarTextInput;
    [SerializeField] private Button searchButton;

    [Header("Main list objects")]
    [SerializeField] private GameObject mainPlayersListObject;
    [SerializeField] private Transform mainPlayersListContent;

    [Header("Recent list objects")]
    [SerializeField] private GameObject recentPlayersListObject;
    [SerializeField] private Transform recentPlayersListContent;

    [Header("Loading Objects")]
    [SerializeField] private GameObject loadingLabelObject;

    [Header("Runtime")]
    [SerializeField] private string searchKeyword = "";

    public void Search()
    {
        if (searchKeyword.Trim().Equals("")) 
            return;

        recentPlayersListObject.SetActive(false);
        loadingLabelObject.SetActive(true);
        mainPlayersListObject.SetActive(false);

        PlayerAccountService.instance.SearchPlayers(
            searchKeyword,
            (response) =>
            {
                ConvertPlayersListResponseToPlayerItem(mainPlayersListContent, response);

                recentPlayersListObject.SetActive(false);
                loadingLabelObject.SetActive(false);
                mainPlayersListObject.SetActive(true);
            },
            (code, error) =>
            {
                Debug.LogError($"[ChallengeWindowUiController] Failed to fetch search players: {error}");
            }
        );
    }

    public void OnKeywordChange(string keyword)
    {
        searchKeyword = keyword;

        if (searchKeyword.Trim().Equals(""))
        {
            recentPlayersListObject.SetActive(true);
            loadingLabelObject.SetActive(false);
            mainPlayersListObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        loadingLabelObject.SetActive(true);

        FriendService.instance.GetRecentPlayers(
            (response) =>
            {
                ConvertPlayersListResponseToPlayerItem(recentPlayersListContent, response);

                recentPlayersListObject.SetActive(true);
                loadingLabelObject.SetActive(false);
                mainPlayersListObject.SetActive(false);
            },
            (code, error) =>
            {
                Debug.LogError($"[ChallengeWindowUiController] Failed to fetch recent players: {error}");
            }
        );
    }

    void ConvertPlayersListResponseToPlayerItem(Transform parent, OtherPlayersListResponse original)
    {
        DestroyAllChildren(parent);

        foreach (OtherPlayerAccountResponse p in original.players)
        {
            GameObject newItem = Instantiate(playerItemPrefab, parent);

            newItem.GetComponent<ChallengePlayerItemUiController>().Initialize(p.username);
        }
    }

    void DestroyAllChildren(Transform parent)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
