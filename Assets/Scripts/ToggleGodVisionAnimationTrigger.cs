using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGodVisionAnimationTrigger : MonoBehaviour
{
    private void Update()
    {
        if (GetComponentInChildren<CameraRotateVertically>().isInGodVisionPosition)
        {
            GetComponent<Animator>().cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }
}
