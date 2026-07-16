using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCooldownBarUiController : MonoBehaviour
{
    private Image fillImage;
    private Coroutine cooldownCoroutine;

    [SerializeField] private Sprite cooldownSprite;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite onGoingSprite;

    void Start()
    {
        fillImage = GetComponent<Image>();
    }

    public void StartDash()
    {
        StopCooldown();
        fillImage.sprite = onGoingSprite;
    }

    public void StartDashCooldown(float duration)
    {
        StopCooldown();
        fillImage.sprite = cooldownSprite;
        cooldownCoroutine = StartCoroutine(AnimateFill(duration));
    }

    public void EndDashCooldown()
    {
        StopCooldown();
        fillImage.fillAmount = 1f;
        fillImage.sprite = readySprite;
    }

    private void StopCooldown()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
            cooldownCoroutine = null;
        }
    }

    private IEnumerator AnimateFill(float duration)
    {
        fillImage.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        fillImage.fillAmount = 1f;
        cooldownCoroutine = null;
    }
}
