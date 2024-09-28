using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRotateVertically : MonoBehaviour
{
    public static CameraRotateVertically instance;

    private Vector3 firstpoint;
    private float firstPointY;
    private Vector3 secondpoint;
    private float secondPointY;
    [HideInInspector] public Animator animator;
    [Header("Eye Icon")]
    [SerializeField] Sprite normalVisionSprite;
    [SerializeField] Sprite godVisionSprite;
    [SerializeField] Button changeVisionButton;

    private Vector3 afterTouuchxVector;
    private Vector3 beforeTouuchxVector;

    public bool canRotateCameraVertically = true;
    [HideInInspector] public bool movingVerticaly = false;
    public bool isInGodVisionPosition = false;

    [SerializeField] float VerticalSpeed;
    [SerializeField] float VerticalComeBackSpeed;
    [SerializeField] GameObject parentCamera;

    private void Start()
    {
        if (instance == null)
            instance = this;
        animator = GetComponent<Animator>();
        beforeTouuchxVector = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    private void Update()
    {
        if (canRotateCameraVertically)
        {
            /*if (isInGodVisionPosition)
            {
                animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            }*/
            foreach (Touch touch in Input.touches)
            {
                if (touch.tapCount == 2 && animator.enabled == false)
                {
                    changeVisionButton.enabled = false;
                    TurnBackCamera();
                }
            }
            if (Input.touchCount > 0)
            {   
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstpoint = Input.GetTouch(0).position;
                    firstPointY = firstpoint.y;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (parentCamera.GetComponent<CameraRotateHorizontaly>().movingHorizontaly == false)
                    {
                        MoveCameraVerticaly();
                    }
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    movingVerticaly = false;
                }   
            }
            else
            {
                if (!isInGodVisionPosition && transform.localPosition.y < 0)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, beforeTouuchxVector, VerticalComeBackSpeed * Time.deltaTime);
            }
        }
    }
    void MoveCameraVerticaly()
    {
        if (!isInGodVisionPosition)
        {
            secondpoint = Input.GetTouch(0).position;
            secondPointY = secondpoint.y;
            float downFactor = (secondPointY - firstPointY) / 100f;
            afterTouuchxVector = new Vector3(transform.localPosition.x, transform.localPosition.y + downFactor, transform.localPosition.z);
            if (afterTouuchxVector.y < -5)
                afterTouuchxVector.y = -5;
            if (afterTouuchxVector.y > 0)
                afterTouuchxVector.y = 0;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, afterTouuchxVector, VerticalSpeed * Time.deltaTime);
            if ((secondpoint.y - firstpoint.y < -20))
                movingVerticaly = true;
        }
    }
    public void ToggleIsInGodVisionMode()
    {
        isInGodVisionPosition = !isInGodVisionPosition;
    }
    public void TurnBackCamera()
    {
        changeVisionButton.enabled = true;
        if (!isInGodVisionPosition)
        {
            animator.enabled = true;
            animator.SetTrigger("GoToGodVision");
            StartCoroutine(TurnOffChildCameraAnimator());
        }
        if (isInGodVisionPosition)
        {
            animator.enabled = true;
            animator.SetTrigger("GoBackToNormalView");
            StartCoroutine(TurnOffChildCameraAnimator());
        }
    }
    IEnumerator TurnOffChildCameraAnimator()
    {
        yield return new WaitForSeconds(1.2f);
        if (!isInGodVisionPosition)
        {
            changeVisionButton.GetComponent<Image>().sprite = normalVisionSprite;
        }
        if (isInGodVisionPosition)
            changeVisionButton.GetComponent<Image>().sprite = godVisionSprite;
        yield return new WaitForSeconds(0.8f);
        animator.enabled = false;
        changeVisionButton.enabled = true;
    }
    public void ComeBackToZeroPos()
    {
        transform.localPosition = beforeTouuchxVector;
    }
}
