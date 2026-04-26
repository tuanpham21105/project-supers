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
    fall,
    strike_attack_1,
    strike_attack_2
}

public enum CharacterUpperAnimation
{
    normal_attack_1,
    normal_attack_2,
    block,
    deflect
}

public class CharacterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionDuration;
    private Coroutine upperLayerFadeCoroutine;
    private int normalAttackComboIndex = 0;
    private List<CharacterUpperAnimation> normalAttackCombo = new List<CharacterUpperAnimation>
    {
        CharacterUpperAnimation.normal_attack_1,
        CharacterUpperAnimation.normal_attack_2
    };

    private int strikeAttackComboIndex = 0;
    private List<CharacterBodyAnimation> strikeAttackCombo = new List<CharacterBodyAnimation>
    {
        CharacterBodyAnimation.strike_attack_1,
        CharacterBodyAnimation.strike_attack_2
    };

    public void PlayBodyAnimation(CharacterBodyAnimation animation, float normalizedTimeOffset = -1f)
    {
        if (animator != null)
        {
            if (normalizedTimeOffset >= 0)
                animator.CrossFade(animation.ToString(), transitionDuration, 0, normalizedTimeOffset);
            else
                animator.CrossFade(animation.ToString(), transitionDuration, 0);
        }
    }

    public void PlayUpperAnimation(CharacterUpperAnimation animation, float speed = 1f)
    {
        if (animator != null)
        {
            animator.SetFloat("DeflectReadySpeed", speed);
            StartFadeUpperLayer(1f, 0.1f);
            animator.CrossFade(animation.ToString(), transitionDuration, 1, 0f);
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

    public void PlayStrikeAttack(bool isContinuing)
    {
        if (isContinuing)
        {
            strikeAttackComboIndex++;
            if (strikeAttackComboIndex >= strikeAttackCombo.Count)
            {
                strikeAttackComboIndex = 0;
            }
        }
        else
        {
            strikeAttackComboIndex = 0;
        }

        PlayBodyAnimation(strikeAttackCombo[strikeAttackComboIndex], 0f);
    }

    public void ResetStrikeAttackCombo()
    {
        strikeAttackComboIndex = 0;
    }

    public void EndUpperAnimation()
    {
        if (animator != null)
        {
            StartFadeUpperLayer(0f, transitionDuration);
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
