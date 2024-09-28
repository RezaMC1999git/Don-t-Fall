using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugCanvasHandler : MonoBehaviour
{
    [SerializeField] CameraRotateHorizontaly parentCamera;
    [SerializeField] TextMeshProUGUI aspectRatioText;
    [SerializeField] TextMeshProUGUI cameraYRotateText;
    [SerializeField] TextMeshProUGUI widthText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] TextMeshProUGUI levelItems;
    LevelParameters levelParameters;
    private void Start()
    {
        levelParameters = FindObjectOfType<FindCurrentLevelParametes>().ConnectLevelParametersToItemSlot();
        aspectRatioText.text = Camera.main.aspect.ToString();
        widthText.text = (Screen.width).ToString();
        heightText.text = (Screen.height).ToString();
        cameraYRotateText.text = parentCamera.transform.localEulerAngles.y.ToString();
        levelItems.text = levelParameters.numberOfAllItems.ToString();
    }
    private void Update()
    {
        levelParameters = FindObjectOfType<FindCurrentLevelParametes>().ConnectLevelParametersToItemSlot();
        levelItems.text = levelParameters.numberOfAllItems.ToString();
        if (parentCamera.transform.localEulerAngles.y / 180 < 1)
        {
            cameraYRotateText.text = (parentCamera.transform.localEulerAngles.y).ToString();
        }
        if (parentCamera.transform.localEulerAngles.y / 180 >= 1)
        {
            cameraYRotateText.text = ((int)parentCamera.transform.localEulerAngles.y - 360f).ToString();
        }
    }
}
