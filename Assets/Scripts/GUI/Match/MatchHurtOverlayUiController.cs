using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHurtOverlayUiController : MonoBehaviour
{
    public static MatchHurtOverlayUiController instance;

    [SerializeField] private GameObject hurtOverlay;

    [SerializeField] private float onTime = 2f;

    private Coroutine showOverlayCoroutine;

    void Awake()
    {
        instance = this;

        gameObject.SetActive(false);
    }

    void Start()
    {
    }

    void OnDestroy()
    {
        instance = null;
    }

    public void PlayerGetHit()
    {
        if (showOverlayCoroutine != null)
            StopCoroutine(showOverlayCoroutine);

        hurtOverlay.SetActive(true);

        showOverlayCoroutine = StartCoroutine(showHurtOverlay());
    }

    IEnumerator showHurtOverlay()
    {
        yield return new WaitForSeconds(onTime);

        hurtOverlay.SetActive(false);

        showOverlayCoroutine = null;
    }
}
