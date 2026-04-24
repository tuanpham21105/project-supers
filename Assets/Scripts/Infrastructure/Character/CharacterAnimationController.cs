using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterBodyAnimation
{
    ground_idle,
    walking_forward,
    walking_backward,
    walking_right,
    walking_left,
    sprint_forward,
    sprint_backward,
    sprint_right,
    sprint_left,
    fly_idle,
    fly_forward,
    fly_backward,
    fly_right,
    fly_left,
    fast_fly,
    jump,
    fall
}

public enum CharacterUpperAnimation
{
    normal_attack_1,
    normal_attack_2
}

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private Coroutine upperLayerFadeCoroutine;
    private int normalAttackComboIndex = 0;
    private List<CharacterUpperAnimation> normalAttackCombo = new List<CharacterUpperAnimation>
    {
        CharacterUpperAnimation.normal_attack_1,
        CharacterUpperAnimation.normal_attack_2
    };

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBodyAnimation(CharacterBodyAnimation animation)
    {
        if (animator != null)
        {
            animator.CrossFade(animation.ToString(), 0.3f, 0);
        }
    }

    public void PlayUpperAnimation(CharacterUpperAnimation animation)
    {
        if (animator != null)
        {
            StartFadeUpperLayer(1f, 0.1f);
            animator.CrossFade(animation.ToString(), 0.1f, 1, 0f);
        }
    }

    public void PlayNormalAttack(bool isContinuing)
    {
        if (isContinuing)
        {
            normalAttackComboIndex++;
            if (normalAttackComboIndex >= normalAttackCombo.Count)
            {
                normalAttackComboIndex = 0;
            }
        }
        else
        {
            normalAttackComboIndex = 0;
        }

        PlayUpperAnimation(normalAttackCombo[normalAttackComboIndex]);
    }

    public void ResetNormalAttackCombo()
    {
        normalAttackComboIndex = 0;
    }

    public void EndUpperAnimation()
    {
        if (animator != null)
        {
            StartFadeUpperLayer(0f, 0.3f);
        }
    }

    private void StartFadeUpperLayer(float targetWeight, float duration)
    {
        if (upperLayerFadeCoroutine != null)
        {
            StopCoroutine(upperLayerFadeCoroutine);
        }
        upperLayerFadeCoroutine = StartCoroutine(FadeLayerWeight(1, targetWeight, duration));
    }

    private IEnumerator FadeLayerWeight(int layerIndex, float targetWeight, float duration)
    {
        float startWeight = animator.GetLayerWeight(layerIndex);
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            animator.SetLayerWeight(layerIndex, Mathf.Lerp(startWeight, targetWeight, elapsed / duration));
            yield return null;
        }

        animator.SetLayerWeight(layerIndex, targetWeight);
        upperLayerFadeCoroutine = null;
    }
}
