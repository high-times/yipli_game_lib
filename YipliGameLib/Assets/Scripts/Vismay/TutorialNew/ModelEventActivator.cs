using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEventActivator : MonoBehaviour
{
    // required variables
    [SerializeField] private ThreeDModelManager threeDModelManager = null;

    public void ActivateWaveAnimator() {
        threeDModelManager.ApplyWavingOverride();
    }

    public void ActivateIdleAnimator() {
        threeDModelManager.ApplyMainIdleOverride();
    }

    public void ActivateWalkingAnimator() {
        threeDModelManager.ApplyWalkingOverride();
    }

    public void ActivateJumpAnimator() {
        threeDModelManager.ApplyJumpOverride();
    }
}
