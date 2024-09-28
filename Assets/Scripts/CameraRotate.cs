using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public static CameraRotate instance;
    private Vector3 firstpoint;
    private float firstPointX;
    private Vector3 secondpoint;
    private float secondPointX;
    private Vector3 afterTouuchxVector;
    private Vector3 beforeTouuchxVector;

    private float yAngle = 0.0f;
    [SerializeField] float VerticalSpeed;
    [SerializeField] float VerticalComeBackSpeed;
    [SerializeField] float HorizontalSpeed;
    public bool isMainCamera;
    public bool canRotateCamera = true;
    public bool movingVerticaly = false;
    public bool movingHorizontaly = false;

    private void Start()
    {
        if (instance == null)
            instance = this;
        beforeTouuchxVector = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    void Update()
    {
        if (canRotateCamera)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstpoint = Input.GetTouch(0).position;
                    firstPointX = firstpoint.y;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    MoveCameras();
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    movingVerticaly = false;
            }
            else
            {
                if (isMainCamera)
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, beforeTouuchxVector, VerticalComeBackSpeed * Time.deltaTime);
            }
        }
    }

    void MoveCameras()
    {
        secondpoint = Input.GetTouch(0).position;
        secondPointX = secondpoint.y;
        float downFactor = (secondPointX - firstPointX) / 100f;
        afterTouuchxVector = new Vector3(transform.localPosition.x, transform.localPosition.y + downFactor, transform.localPosition.z);
        if (afterTouuchxVector.y < -5)
            afterTouuchxVector.y = -5;
        if (afterTouuchxVector.y > 0)
            afterTouuchxVector.y = 0;

        if (isMainCamera && Camera.main.GetComponentInParent<CameraRotate>().movingHorizontaly == false)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, afterTouuchxVector, VerticalSpeed * Time.deltaTime);
            if(transform.localPosition.y < 0)
                movingVerticaly = true;
        }
        else if(Camera.main.GetComponent<CameraRotate>().movingVerticaly == false)
        {
            yAngle = ((secondpoint.x - firstpoint.x) * 180.0f / Screen.width) + transform.localEulerAngles.y;
            this.transform.rotation = Quaternion.RotateTowards(transform.localRotation,
                Quaternion.Euler(0f, yAngle, transform.localEulerAngles.z), HorizontalSpeed);

        }
    }
}
