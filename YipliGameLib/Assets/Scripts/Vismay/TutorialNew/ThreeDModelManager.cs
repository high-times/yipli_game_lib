using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDModelManager : MonoBehaviour
{
    // required variables
    [Header("3d Character")]
    [SerializeField] private GameObject mainCharacter = null;
    [SerializeField] private GameObject modelManager = null;
    [SerializeField] private Animator mainCharacterAnimator = null;

    [Header("All Override controllers")]
    [SerializeField] private AnimatorOverrideController leftTapController = null;
    [SerializeField] private AnimatorOverrideController rightTapController = null;
    [SerializeField] private AnimatorOverrideController jumpController = null;
    [SerializeField] private AnimatorOverrideController idleController = null;
    [SerializeField] private AnimatorOverrideController walkingController = null;
    [SerializeField] private AnimatorOverrideController wavingController = null;
    [SerializeField] private AnimatorOverrideController headNodController = null;
    [SerializeField] private AnimatorOverrideController fistPumpController = null;

    void Start() {
        DisableModelManagerAnimator();
        Hide3DModel();
    }

    public void ApplyLeftTapOverride() {
        mainCharacterAnimator.runtimeAnimatorController = leftTapController;
    }

    public void ApplyRightTapOverride() {
        mainCharacterAnimator.runtimeAnimatorController = rightTapController;
    }

    public void ApplyJumpOverride() {
        mainCharacterAnimator.runtimeAnimatorController = jumpController;
    }

    public void ApplyWalkingOverride() {
        mainCharacterAnimator.runtimeAnimatorController = walkingController;
    }

    public void ApplyWavingOverride() {
        mainCharacterAnimator.runtimeAnimatorController = wavingController;
    }

    public void ApplyMainIdleOverride() {
        mainCharacterAnimator.runtimeAnimatorController = idleController;
    }

    public void ApplyHeadNodOverride() {
        mainCharacterAnimator.runtimeAnimatorController = headNodController;
    }

    public void ApplyFistPumpOverride() {
        mainCharacterAnimator.runtimeAnimatorController = fistPumpController;
    }

    public void Display3DModel() {
        mainCharacter.SetActive(true);
    }

    public void Hide3DModel() {
        mainCharacter.SetActive(false);
    }

    public void EnableModelManagerAnimator() {
        modelManager.GetComponent<Animator>().enabled = true;
    }

    public void DisableModelManagerAnimator() {
        modelManager.GetComponent<Animator>().enabled = false;
    }

    // pausePart, resumePart

    public void ActivatePausePart() {
        EnableModelManagerAnimator();
        modelManager.GetComponent<Animator>().SetBool("pausePart", true);
    }

    public void ActivateResumePart() {
        EnableModelManagerAnimator();
        modelManager.GetComponent<Animator>().SetBool("resumePart", true);
    }

    public void ResetAllTriggers() {
        modelManager.GetComponent<Animator>().SetBool("pausePart", false);
        modelManager.GetComponent<Animator>().SetBool("resumePart", false);

        DisableModelManagerAnimator();
    }
}
