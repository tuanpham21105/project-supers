using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOverlayController : MonoBehaviour
{
    public static MainMenuOverlayController instance;
    
    [SerializeField] private GameObject errorPopup;
    [SerializeField] private GameObject connectingOverlay;
    [SerializeField] private GameObject matchReadyOverlay;

    [SerializeField] private GameObject currentOpen;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MatchMakingController.instance.onPeerConnecting += handleOnConnecting;
        MatchMakingController.instance.onPeerConnected += handleOnMatchReady;
    }

    private void OnEnable()
    {
        Application.logMessageReceived += OnLogReceived;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogReceived;
    }

    private void OnDestroy()
    {
        instance = null;
        
        Application.logMessageReceived -= OnLogReceived;

        if (MatchMakingController.instance != null)
        {
            MatchMakingController.instance.onPeerConnecting -= handleOnConnecting;
            MatchMakingController.instance.onPeerConnected -= handleOnMatchReady;
        }
    }

    public void OpenOverlay(GameObject overlay)
    {
        if (!currentOpen.IsUnityNull())
            CloseOverlay();

        currentOpen = overlay;

        currentOpen.GetComponent<WindowUiController>().OpenWindow();
        gameObject.GetComponent<Image>().raycastTarget = true;
    }

    public void CloseOverlay()
    {
        currentOpen.GetComponent<WindowUiController>().CloseWindow();
            gameObject.GetComponent<Image>().raycastTarget = false;

            currentOpen = null;
    }

    private void OnLogReceived(
        string condition,
        string stackTrace,
        LogType type)
    {
        if (type == LogType.Error)
        {
            errorPopup.GetComponent<ErrorPopupUiController>().SetError(condition);
            OpenOverlay(errorPopup);
        }
    }

    void handleOnConnecting()
    {
        OpenOverlay(connectingOverlay);
    }

    void handleOnMatchReady()
    {
        OpenOverlay(matchReadyOverlay);
    }
}
