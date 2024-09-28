using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuVerticalMove : MonoBehaviour
{
    private Vector3 firstpoint;
    private float firstPointY;
    private Vector3 secondpoint;
    private float secondPointY;
    private Vector3 afterTouuchxVector;
    private Vector3 normalPos = new Vector3(0f,12f,0f);
    public Vector3 beforeStartLevelPos;
    [SerializeField] float verticalGoUpSpeed;
    [SerializeField] float backToNormalSpeed;
    public float upCameraLimit = 20f;
    public float downCameraLimit = 0f;
    public bool shouldGoToNormal = false;
    public bool shouldGoToPreviousPos = false;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                firstpoint = Input.GetTouch(0).position;
                firstPointY = firstpoint.y;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                //MoveCameraUp();
            }
        }
        if (shouldGoToNormal)
        {
            if (transform.position == normalPos)
                shouldGoToNormal = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, normalPos, backToNormalSpeed * Time.deltaTime);
        }
        if (shouldGoToPreviousPos)
        {
            if (transform.position == beforeStartLevelPos)
                shouldGoToPreviousPos = false;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, beforeStartLevelPos, backToNormalSpeed * Time.deltaTime);
        }
    }
    void MoveCameraUp()
    {
        secondpoint = Input.GetTouch(0).position;
        secondPointY = secondpoint.y;
        float downFactor = (firstPointY - secondPointY) / 100f;
        afterTouuchxVector = new Vector3(transform.localPosition.x, transform.localPosition.y + downFactor, transform.localPosition.z);
        if (afterTouuchxVector.y < downCameraLimit)
            afterTouuchxVector.y = downCameraLimit;
        if (afterTouuchxVector.y > upCameraLimit)
            afterTouuchxVector.y = upCameraLimit;
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, afterTouuchxVector, verticalGoUpSpeed * Time.deltaTime);
    }
    public bool IsCameraPositionRight()
    {
        bool rightOrNot = false;
        if (transform.position == beforeStartLevelPos)
            rightOrNot = true;
        return rightOrNot;
    }
}
