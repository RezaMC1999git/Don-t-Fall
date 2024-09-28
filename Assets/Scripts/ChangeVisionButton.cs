using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVisionButton : MonoBehaviour
{
    [SerializeField] CameraRotateVertically childCamera;
    [SerializeField] Sprite normalVisionSprite;
    [SerializeField] Sprite godVisionSprite;
    private void OnEnable()
    {
        GetComponent<Button>().interactable = true;
        if (!childCamera.isInGodVisionPosition)
            GetComponent<Image>().sprite = normalVisionSprite;
        if (childCamera.isInGodVisionPosition)
            GetComponent<Image>().sprite = godVisionSprite;
    }
    public void ChangeVision()
    {
        GetComponent<Button>().interactable = false;
        if (!childCamera.isInGodVisionPosition)
        {
            childCamera.animator.enabled = true;
            childCamera.animator.SetTrigger("GoToGodVision");
            StartCoroutine(TurnOffChildCameraAnimator());
        }
        if (childCamera.isInGodVisionPosition)
        {
            childCamera.animator.enabled = true;
            childCamera.animator.SetTrigger("GoBackToNormalView");
            StartCoroutine(TurnOffChildCameraAnimator());
        }
    }
    IEnumerator TurnOffChildCameraAnimator()
    {
        yield return new WaitForSeconds(1.2f);
        if (!childCamera.isInGodVisionPosition)
            GetComponent<Image>().sprite = normalVisionSprite;
        if (childCamera.isInGodVisionPosition)
            GetComponent<Image>().sprite = godVisionSprite;
        yield return new WaitForSeconds(0.8f);
        childCamera.animator.enabled = false;
        GetComponent<Button>().interactable = true;
    }
}
