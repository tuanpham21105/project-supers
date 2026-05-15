using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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
    strike_attack_2,
    knock_out_1,
    knock_out_2,
    dead_1,
    dead_2
}

public enum CharacterUpperAnimation
{
    none,
    normal_attack_1,
    normal_attack_2,
    block,
    deflect,
    deflected,
}

public enum AdditionalAnimation
{
    none,
    hit,
    hit_1,
    hit_2
}

public class CharacterAnimationController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterObjectsData characterObjectsData;
    private CharacterStatesData characterStatesData;
    private Animator animator;

    // [Constant]
    [Header("Constant")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private List<CharacterUpperAnimation> normalAttackCombo = new List<CharacterUpperAnimation>
    {
        CharacterUpperAnimation.normal_attack_1,
        CharacterUpperAnimation.normal_attack_2
    };
    [SerializeField] private List<CharacterBodyAnimation> strikeAttackCombo = new List<CharacterBodyAnimation>
    {
        CharacterBodyAnimation.strike_attack_1,
        CharacterBodyAnimation.strike_attack_2
    };
    [SerializeField] private List<AdditionalAnimation> hitAnimations = new List<AdditionalAnimation>
    {
        AdditionalAnimation.hit,
        AdditionalAnimation.hit_1,
        AdditionalAnimation.hit_2
    };

    // [Runtime]
    [Header("Runtime")]
    private Coroutine upperLayerFadeCoroutine;

    void Start()
    {
        characterObjectsData = GetComponent<CharacterObjectsData>();
        characterStatesData = GetComponent<CharacterStatesData>();

        animator = characterObjectsData.characterMesh.GetComponent<Animator>();
    }

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
            StartFadeUpperLayer(1f, transitionDuration);
            animator.CrossFade(animation.ToString(), transitionDuration, 1, 0f);
        }
    }

    public void PlayAdditionalAnimation(AdditionalAnimation animation)
    {
        if (animator != null)
        {
            animator.SetLayerWeight(2, 0.5f);
            animator.CrossFade(animation.ToString(), transitionDuration, 2, 0f);
        }
    }

    public void PlayHitAnimation()
    {
        PlayAdditionalAnimation(hitAnimations[characterStatesData.hitAnimationIndex]);
        characterStatesData.hitAnimationIndex++;
        if (characterStatesData.hitAnimationIndex >= hitAnimations.Count)
        {
            characterStatesData.hitAnimationIndex = 0;
        }
    }

    public void PlayDeflectedAnimation()
    {
        PlayUpperAnimation(CharacterUpperAnimation.deflected);
    }

    public void PlayNormalAttack(bool isContinuing)
    {
        if (isContinuing)
        {
            characterStatesData.normalAttackComboIndex++;
            if (characterStatesData.normalAttackComboIndex >= normalAttackCombo.Count)
            {
                characterStatesData.normalAttackComboIndex = 0;
            }
        }
        else
        {
            characterStatesData.normalAttackComboIndex = 0;
        }

        PlayUpperAnimation(normalAttackCombo[characterStatesData.normalAttackComboIndex]);
    }

    public void ResetNormalAttackCombo()
    {
        characterStatesData.normalAttackComboIndex = 0;
    }

    public void PlayStrikeAttack(bool isContinuing)
    {
        if (isContinuing)
        {
            characterStatesData.strikeAttackComboIndex++;
            if (characterStatesData.strikeAttackComboIndex >= strikeAttackCombo.Count)
            {
                characterStatesData.strikeAttackComboIndex = 0;
            }
        }
        else
        {
            characterStatesData.strikeAttackComboIndex = 0;
        }

        PlayBodyAnimation(strikeAttackCombo[characterStatesData.strikeAttackComboIndex], 0f);
    }

    public void ResetStrikeAttackCombo()
    {
        characterStatesData.strikeAttackComboIndex = 0;
    }

    public void PlayKnockOutAnimation(bool front)
    {
        CharacterBodyAnimation animation = front ? CharacterBodyAnimation.knock_out_1 : CharacterBodyAnimation.knock_out_2;
        PlayBodyAnimation(animation);
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
