using System.Collections;
using UnityEngine;

public class CharacterDebuffController : MonoBehaviour
{
    [SerializeField] private CharacterObjectsData characterObjectsData;
    [SerializeField] private CharacterStatesData characterStatesData;
    [SerializeField] private CharacterMovementController characterMovementController;
    [SerializeField] private CharacterAnimationController animationController;
    private CharacterObjectService characterObjectService;
    private CharacterAnimationEvents animationEvents;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();
        if (characterMovementController == null) characterMovementController = GetComponent<CharacterMovementController>();
        if (animationController == null) animationController = GetComponent<CharacterAnimationController>();
        if (characterObjectService == null) characterObjectService = GetComponent<CharacterObjectService>();

        if (characterObjectsData != null && characterObjectsData.characterMesh != null)
        {
            animationEvents = characterObjectsData.characterMesh.GetComponent<CharacterAnimationEvents>();
            if (animationEvents != null)
            {
                animationEvents.OnHitEnd += HandleHitEnd;
                animationEvents.OnDeflectedEnd += HandleDeflectedEnd;
            }
        }

        if (characterObjectService != null)
        {
            characterObjectService.OnImpactForceDecayed += EndKnockOut;
            characterObjectService.OnImpactCollision += HandleImpactCollision;
        }
    }

    private void OnDestroy()
    {
        if (animationEvents != null)
        {
            animationEvents.OnHitEnd -= HandleHitEnd;
            animationEvents.OnDeflectedEnd -= HandleDeflectedEnd;
        }

        if (characterObjectService != null)
        {
            characterObjectService.OnImpactForceDecayed -= EndKnockOut;
            characterObjectService.OnImpactCollision -= HandleImpactCollision;
        }
    }
    
    public void Hit(Vector3 direction)
    {
        if (characterStatesData != null && characterStatesData.hitFlag) return;

        if (characterStatesData != null) characterStatesData.hitFlag = true;
        if (animationController != null)
        {
            animationController.PlayHitAnimation();
        }
    }

    private void HandleHitEnd()
    {
        if (characterStatesData != null) characterStatesData.hitFlag = false;
        if (animationController != null)
        {
            animationController.PlayAdditionalAnimation(AdditionalAnimation.none);
        }
    }

    private void HandleImpactCollision()
    {
        StartCoroutine(ImpactCollisionRoutine());
    }

    private IEnumerator ImpactCollisionRoutine()
    {
        yield return new WaitForSeconds(1f);
        EndKnockOut();
    }

    private void EndKnockOut()
    {
        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.none);

            // if (animationController != null)
            // {
            //     animationController.PlayBodyAnimation(characterStatesData.flyFlag ? CharacterBodyAnimation.fly_idle : CharacterBodyAnimation.ground_idle);
            // }
        }

        characterMovementController.ForceRefreshBodyAnimation();
    }

    public void KnockBack(Vector3 knockbackForce)
    {
        if (characterObjectService != null)
        {
            characterObjectService.ApplyForce(knockbackForce);
        }
    }

    public void KnockOut(Vector3 direction, bool isFront)
    {
        if (characterStatesData != null && characterStatesData.knockAwayFlag) return;

        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.knock_out);
            characterStatesData.knockAwayFlag = true;
            characterStatesData.bodyActionFlag = true;
            characterStatesData.upperActionFlag = true;
        }

        if (animationController != null)
        {
            animationController.EndUpperAnimation();
            animationController.PlayKnockOutAnimation(isFront);
        }

        if (characterObjectService != null)
        {
            characterObjectService.SetDirection(direction * (isFront ? -1 : 1));
            characterObjectService.ApplyForce(direction);
        }
    }

    public void Deflected(Vector3 direction)
    {
        if (characterStatesData != null && characterStatesData.deflectedFlag) return;

        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.deflected);
            characterStatesData.deflectedFlag = true;
            characterStatesData.upperActionFlag = true;
        }
        
        if (animationController != null)
        {
            animationController.PlayDeflectedAnimation();
        }
    }

    private void HandleDeflectedEnd()
    {
        if (characterStatesData != null)
        {
            if (characterStatesData.currentProcessAction != CharacterProcessAction.deflected) return;
            characterStatesData.ChangeProcessAction(CharacterProcessAction.none);
        }
        
        if (animationController != null)
        {
            animationController.EndUpperAnimation();
        }
    }
}
