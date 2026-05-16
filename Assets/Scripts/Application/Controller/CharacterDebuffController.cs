using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterDebuffController : MonoBehaviour
{
    // [Dependencies]
    [Header("Dependencies")]
    private CharacterObjectsData characterObjectsData;
    private CharacterStatesData characterStatesData;
    private CharacterStatsData characterStatsData;
    private CharacterMovementController characterMovementController;
    private CharacterAnimationController animationController;
    private CharacterObjectService characterObjectService;
    private CharacterAnimationEvents animationEvents;

    void Start()
    {
        if (characterObjectsData == null) characterObjectsData = GetComponent<CharacterObjectsData>();
        if (characterStatesData == null) characterStatesData = GetComponent<CharacterStatesData>();
        if (characterStatsData == null) characterStatsData = GetComponent<CharacterStatsData>();
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

        characterMovementController.endFlying += HandleEndFallKnockOut;
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

        characterMovementController.endFlying -= HandleEndFallKnockOut;
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

            if (characterStatesData.currentEndurance <= 0)
            {
                if (characterStatesData.fallFlag)
                {
                    FallKnockOut();
                }
                else 
                    Dead();
            }
        }

        characterMovementController.ForceRefreshBodyAnimation();
    }

    public void KnockBack(Vector3 direction, int damage)
    {
        if (characterObjectService != null)
        {
            Vector3 force = direction * damage * characterStatsData.damageKnockAwayRatio;
            characterObjectService.ApplyForce(force);
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

    private void HandleEndFallKnockOut()
    {
        if (characterStatesData.knockAwayFlag && characterStatesData.currentEndurance <= 0)
        {
            Dead();
        } 
    }
    // [Control methods]
    public void Hit(Vector3 direction, int damage)
    {
        if (animationController != null && !characterStatesData.hitFlag)
        {
            animationController.PlayHitAnimation();
        }
        
        if (characterStatesData != null) characterStatesData.hitFlag = true;

        KnockBack(direction, damage);
    }

    public void KnockOut(Vector3 direction, bool isFront, int damage)
    {
        // if (characterStatesData != null && characterStatesData.knockAwayFlag) return;

        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.knock_out);
            characterStatesData.knockAwayFlag = true;
            characterStatesData.bodyActionFlag = true;
            characterStatesData.upperActionFlag = true;

            if (characterStatesData.currentEndurance <= 0)
            {
                characterMovementController.SetFlyEnd();
            }

            characterStatesData.isFront = isFront;
        }

        if (animationController != null)
        {
            animationController.EndUpperAnimation();
            animationController.PlayKnockOutAnimation(isFront);
        }

        if (characterObjectService != null)
        {
            characterObjectService.SetDirection(direction * (isFront ? -1 : 1));
            KnockBack(direction, damage);
        }
    }

    public void FallKnockOut()
    {
        Vector3 direction = Vector3.down;
        bool isFront = characterObjectService.IsFaceUp();

        // if (characterStatesData != null && characterStatesData.knockAwayFlag) return;

        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.knock_out);
            characterStatesData.knockAwayFlag = true;
            characterStatesData.bodyActionFlag = true;
            characterStatesData.upperActionFlag = true;

            characterStatesData.isFront = isFront;
        }

        if (animationController != null)
        {
            animationController.EndUpperAnimation();
            animationController.PlayKnockOutAnimation(isFront);
        }

        if (characterObjectService != null)
        {
            // Last horizontal facing direction
            Vector3 horizontal = characterObjectsData.characterObject.transform.forward * (isFront ? -1 : 1);
            horizontal.y = 0f;
            horizontal.Normalize();

            // Body forward when lying down
            Vector3 forward = (isFront ? Vector3.up : Vector3.down);

            // Build rotation:
            // forward = up/down
            // up = previous horizontal direction
            Quaternion rot = Quaternion.LookRotation(forward, horizontal);

            characterObjectsData.characterObject.transform.rotation = rot;
        }
    }

    public void Deflected()
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

    public void Dead()
    {
        if (characterStatesData != null)
        {
            characterStatesData.ChangeProcessAction(CharacterProcessAction.dead);
            characterStatesData.deadFlag = true;
            characterStatesData.bodyActionFlag = true;
        }

        if (characterObjectService != null)
        {
            // Last horizontal facing direction
            Vector3 horizontal = characterObjectsData.characterObject.transform.up * (characterStatesData.isFront ? -1 : 1);
            horizontal.y = 0f;
            horizontal.Normalize();

            characterObjectService.SetDirection(horizontal);
        }

        if (animationController != null)
        {
            animationController.PlayBodyAnimation(characterStatesData.isFront ? CharacterBodyAnimation.dead_1 : CharacterBodyAnimation.dead_2);
        }
    }
}
