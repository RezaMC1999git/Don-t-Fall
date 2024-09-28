using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    Vector3 touchStart;
    [HideInInspector] public GameObject currentItem;
    CameraRotateVertically childCamera;
    CameraRotateHorizontaly parentCamera;
    [SerializeField] float minZoom = 55f;
    [SerializeField] float maxZoom = 85f;
    [SerializeField] float zoomSpeed = 0.01f;
    private void Start()
    {
        childCamera = Camera.main.GetComponent<CameraRotateVertically>();
        parentCamera = GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<CameraRotateHorizontaly>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            parentCamera.canRotateCameraHorizontaly = false;
            childCamera.canRotateCameraVertically = false;

            float difference = currentMagnitude - prevMagnitude;
            Zoom(difference * zoomSpeed);
        }
        else if (currentItem == null)
        {
            parentCamera.canRotateCameraHorizontaly = true;
            childCamera.canRotateCameraVertically = true;
        }
    }
    void Zoom(float increment)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment , minZoom , maxZoom);
    }
}