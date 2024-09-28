using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    public int unlockedLevel = 1;
    public int currentLevel = 1;
    [SerializeField] int currentDiskIndex = 0;

    public GameObject parentCamera;
    public GameObject childCamera;
    public GameObject itemSlotsCanvas;
    public GameObject debugCanvasCanvas;
    public GameObject levelNamecanvas;
    public GameObject menuCanvas;
    public GameObject winPanel;
    public GameObject startLevel;
    public TextMeshProUGUI levelNumber;
    public GameObject pauseCanvas;
    public Transform ItemsParent;
    public LevelParameters levelParameters;

    public GameObject[] multipleItemSlots; //4 ItemSlots - 5 ItemSlots - 6 ItemSlots
    public GameObject[] disks;
    public bool isInsideLevel = false;
    public string[] levelNames;
    private void Start()
    {
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
    }
    private void Update()
    {
        if (Input.touchCount > 0)
            SelectLevel();
    }

    public void YesButton()
    {
        disks[currentDiskIndex].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        startLevel.SetActive(false);
        SelectedDisk();
    }
    public void NoButton()
    {
        Time.timeScale = 1f;
        startLevel.SetActive(false);
    }
    void SelectLevel()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Detect What Item Are We Touching
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            //Instantiate Item
            if (results.Capacity > 0)
            {
                if (results[0].gameObject.GetComponent<Disk>() != null)
                {
                    if(results[0].gameObject.GetComponent<Disk>().diskLevelNumber <= unlockedLevel)
                    {
                        currentLevel = results[0].gameObject.GetComponent<Disk>().diskLevelNumber;
                        currentDiskIndex = currentLevel - 1;
                        Time.timeScale = 0f;
                        levelNumber.text = currentLevel.ToString();
                        startLevel.SetActive(true);
                    }
                }
            }
        }
    }
    void SelectedDisk()
    {
        Time.timeScale = 1f;
        childCamera.GetComponent<PhysicsRaycaster>().enabled = false;
        //Set Other Disks Out Of Reach
        if (currentLevel > 1) // For Levels More Than 1
        {
            for (int i = currentLevel; i < disks.Length; i++)
            {
                disks[i].GetComponentInParent<Animator>().SetTrigger("DiskFlyUp");
            }
            for(int j = currentLevel -2 ; j >= 0; j--)
            {
                disks[j].GetComponentInParent<Animator>().SetTrigger("GoAway");
            }
        }
        else //For Level 1
        {
            for (int i = currentLevel; i < disks.Length; i++)
            {
                disks[i].GetComponentInParent<Animator>().SetTrigger("DiskFlyUp");
            }
        }
        //Set The Current Disk
        StartCoroutine(StartLevel());
    }
    IEnumerator StartLevel()
    {
        isInsideLevel = true;
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        levelParameters.callculatenumberOfAllItems();
        levelParameters.isLevelInProgress = true;
        parentCamera.GetComponent<MainMenuVerticalMove>().beforeStartLevelPos = parentCamera.transform.position;
        menuCanvas.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        disks[currentDiskIndex].GetComponentInParent<Animator>().SetTrigger("LevelStarted");
        parentCamera.GetComponent<MainMenuVerticalMove>().shouldGoToNormal = true;
        
        yield return new WaitForSeconds(1.5f);
        disks[currentDiskIndex].GetComponentInParent<Animator>().enabled = false;
        disks[currentDiskIndex].GetComponent<Rigidbody>().useGravity = true;
        disks[currentDiskIndex].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        //Set Level Text 
        levelNamecanvas.SetActive(true);
        levelNamecanvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentLevel.ToString();
        levelNamecanvas.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = levelNames[currentLevel -1].ToString();

        //DeActivate Camera FlyUp
        parentCamera.GetComponent<CameraRotateHorizontaly>().enabled = true;
        parentCamera.GetComponent<CameraZoom>().enabled = true;
        childCamera.GetComponent<CameraRotateVertically>().enabled = true;

        parentCamera.GetComponent<MainMenuVerticalMove>().enabled = false;
        if(currentLevel<=3)
            yield return new WaitForSeconds(1.5f);
        if(currentLevel>3)
            yield return new WaitForSeconds(1.8f);
        parentCamera.GetComponent<MainMenuVerticalMove>().shouldGoToNormal = false;
        parentCamera.GetComponent<MainMenuVerticalMove>().enabled = false;
        levelNamecanvas.SetActive(false);
        itemSlotsCanvas.SetActive(true);
        pauseCanvas.SetActive(true);

        switch (levelParameters.numberOfItem.Length)
        {
            case 4:
                multipleItemSlots[0].SetActive(true);
                //Find Relative Items
                foreach (GameObject itemslot in multipleItemSlots[0].GetComponent<ItemSlotsHandler>().slots)
                {
                    itemslot.GetComponent<ItemSlot>().Initialize();
                }
                break;
            case 5:
                multipleItemSlots[1].SetActive(true);
                //Find Relative Items
                foreach (GameObject itemslot in multipleItemSlots[1].GetComponent<ItemSlotsHandler>().slots)
                {
                    itemslot.GetComponent<ItemSlot>().Initialize();
                }
                break;
            case 6:
                multipleItemSlots[2].SetActive(true);
                //Find Relative Items
                foreach (GameObject itemslot in multipleItemSlots[2].GetComponent<ItemSlotsHandler>().slots)
                {
                    itemslot.GetComponent<ItemSlot>().Initialize();
                }
                break;
        }
        disks[currentDiskIndex].GetComponent<TimeBody>().CalculateTime();
        //debugCanvasCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        isInsideLevel = false;
        levelParameters.isLevelInProgress = false;
        childCamera.GetComponent<CameraRotateVertically>().ComeBackToZeroPos();
        disks[currentDiskIndex].GetComponent<TimeBody>().continueCallculating = false;
        disks[currentDiskIndex].GetComponent<TimeBody>().timeElapsed = 0f;
        parentCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 2.5f);
        parentCamera.GetComponent<MainMenuVerticalMove>().shouldGoToPreviousPos = true;
        foreach (Transform item in ItemsParent)
        {
            Destroy(item.gameObject);
        }
        for (int k = currentDiskIndex; k >= 0; k--)
        {
            disks[k].transform.rotation = Quaternion.EulerAngles(0f, 0f, 0f);
            disks[k].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        winPanel.SetActive(false);
        itemSlotsCanvas.SetActive(false);
        
        switch (levelParameters.numberOfItem.Length)
        {
            case 4:
                multipleItemSlots[0].SetActive(false);
                break;
            case 5:
                multipleItemSlots[1].SetActive(false);
                break;
            case 6:
                multipleItemSlots[2].SetActive(false);
                break;
        }
        //debugCanvasCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        if (childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
            childCamera.GetComponent<CameraRotateVertically>().TurnBackCamera();
        //{
            //StartCoroutine(HandleGodVision());
        //}
        //Set All Disks To Right Positions
        disks[currentDiskIndex].GetComponent<Rigidbody>().useGravity = false;
        disks[currentDiskIndex].GetComponentInParent<Animator>().enabled = true;
        disks[currentDiskIndex].GetComponentInParent<Animator>().SetTrigger("LevelFinished");
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        if (currentLevel > 1) // For Levels More Than 1
        {
            for (int i = currentLevel; i < disks.Length; i++)
            {
                disks[i].GetComponentInParent<Animator>().enabled = true;
                disks[i].GetComponentInParent<Animator>().SetTrigger("DiskFlyDown");
            }
            for (int j = currentLevel - 2; j >= 0; j--)
            {
                disks[j].GetComponentInParent<Animator>().SetTrigger("ComeBack");
            }
        }
        else //For Level 1
        {
            for (int i = currentLevel; i < disks.Length; i++)
            {
                disks[i].GetComponentInParent<Animator>().SetTrigger("DiskFlyDown");
            }
        }
        //Set Right Camera Scripts
        parentCamera.GetComponent<CameraRotateHorizontaly>().enabled = false;
        parentCamera.GetComponent<CameraZoom>().enabled = false;
        childCamera.GetComponent<CameraRotateVertically>().enabled = false;

        parentCamera.GetComponent<MainMenuVerticalMove>().enabled = true;
        if (unlockedLevel<5)
            unlockedLevel++;
        if (levelParameters.DoesLevelIncludeSlipperyPart)
            levelParameters.ResetSlipperyMaterials();
        menuCanvas.SetActive(true);
        childCamera.GetComponent<PhysicsRaycaster>().enabled = true;

        //if (unlockedLevel > 5)
        //    unlockedLevel = 1;
        //currentDiskIndex++;
        //disks[0].GetComponentInParent<Animator>().enabled = false;
    }
    IEnumerator HandleGodVision()
    {
        childCamera.GetComponent<Animator>().enabled = true;
        childCamera.GetComponent<Animator>().SetTrigger("GoBackToNormalView");
        yield return new WaitForSeconds(2f);
        childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition = false;
        childCamera.GetComponent<Animator>().enabled = false;
    }
    IEnumerator TurnOnMenuNavigatorAgain()
    {
        yield return new WaitForSeconds(1f);
        menuCanvas.SetActive(true);
    }
}
