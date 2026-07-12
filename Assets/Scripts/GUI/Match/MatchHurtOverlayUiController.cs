using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHurtOverlayUiController : MonoBehaviour
{
    public static MatchHurtOverlayUiController instance;

    [SerializeField] private GameObject hurtOverlay;

    [SerializeField] private float onTime = 2f;

    private Coroutine showOverlayCoroutine;

    void Start()
    {
        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void PlayerGetHit()
    {
        if (showOverlayCoroutine != null)
            StopCoroutine(showOverlayCoroutine);

        showOverlayCoroutine = StartCoroutine(showHurtOverlay());
    }

    IEnumerator showHurtOverlay()
    {
        hurtOverlay.SetActive(true);

        yield return new WaitForSeconds(onTime);

        hurtOverlay.SetActive(false);

        showOverlayCoroutine = null;
    }
}
