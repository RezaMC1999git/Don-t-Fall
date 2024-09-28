using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateHorizontaly : MonoBehaviour
{
    public static CameraRotateHorizontaly instance;

    private Vector3 firstpoint;
    private Vector3 secondpoint;

    private float xAngle = 0.0f;

    [HideInInspector] public bool canRotateCameraHorizontaly = true;
    [HideInInspector] public bool canGoToGodVision = false;
    [HideInInspector] public bool movingHorizontaly = false;

    [SerializeField] float HorizontalSpeed;
    [SerializeField] Camera childCamera;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }
    private void Update()
    {
        if (canRotateCameraHorizontaly)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    firstpoint = Input.GetTouch(0).position;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (childCamera.GetComponent<CameraRotateVertically>().movingVerticaly == false)
                        MoveCameraHorizontaly();
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    movingHorizontaly = false;
                }
            }
        }
    }
    void MoveCameraHorizontaly()
    {
        secondpoint = Input.GetTouch(0).position;
        if (secondpoint.x - firstpoint.x < -35 || secondpoint.x - firstpoint.x > 30)
        {
            if(secondpoint.y - firstpoint.y < 50)
                movingHorizontaly = true;
        }
        xAngle = ((secondpoint.x - firstpoint.x) * 180.0f / Screen.width) + transform.localEulerAngles.y;
        this.transform.rotation = Quaternion.RotateTowards(transform.localRotation,
            Quaternion.Euler(0f, xAngle, transform.localEulerAngles.z), HorizontalSpeed * Time.deltaTime);
    }
}
