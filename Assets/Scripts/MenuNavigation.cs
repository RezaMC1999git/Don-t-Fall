using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    public Slider menuSlider;
    public GameObject parentCamera;
    public float maxParentCameraLimit = 22f;
    public float minParentCameraLimit = 12f;
    public bool needToBeActivated = true;
    MainMenuVerticalMove mainMenuVerticalMove;
    private void Start()
    {
        mainMenuVerticalMove = FindObjectOfType<MainMenuVerticalMove>();
        menuSlider.minValue = minParentCameraLimit;
        menuSlider.maxValue = maxParentCameraLimit;
    }
    private void OnDisable()
    {
        needToBeActivated = false;
    }
    private void Update()
    {
        if (mainMenuVerticalMove.IsCameraPositionRight() == true)
            needToBeActivated = true;
        if (needToBeActivated)
        {
            Vector3 temp = parentCamera.transform.localPosition;
            temp.y = menuSlider.value;
            parentCamera.transform.localPosition = temp;
        }
    }
}
