using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBehaviour : MonoBehaviour
{
    CameraRotateVertically childCamera;
    [SerializeField] CameraRotateHorizontaly parentCamera;
    GameObject CreatedItem;
    Vector3 touchPos;
    public Vector3 improvedTouchPos;
    [HideInInspector] public bool CanControllItem = true;
    private bool isItemDropped = false;

    [HideInInspector] public float UPxsmooth;
    [HideInInspector] public float DOWNxsmooth;
    [HideInInspector] public float UPzsmooth;
    [HideInInspector] public float DOWNzsmooth;

    [HideInInspector] public float upxChangeFactor = 0.25f;
    [HideInInspector] public float upzChangeFactor = 0.25f;
    [HideInInspector] public float downChangeFactor = 0.25f;

    public float holdItemHigherBelowSurface = 3.5f;
    public Sprite itemPreviewSprite;
    public GameObject dropVFX;
    public bool isItemOnTopOfSurface = false;
    public bool isOutOfBounds = false;
    [HideInInspector] public bool isLastItem = false;
    public bool itemTouchedDisk = false;
    public bool itemTouchedAnotherItem = false;

    [HideInInspector] public List<GameObject> collisions = new List<GameObject>();

    List<RaycastResult> results;
    PointerEventData eventDataCurrentPosition;

    CameraZoom cameraZoom;
    [HideInInspector] bool canReturnToSlot = false;
    [HideInInspector] public LevelParameters levelParameters;
    [SerializeField] ItemSlot itemSlot;
    public ParticleSystem deathParticle;
    public float holdItemHigher = 1.5f;
    [HideInInspector] public bool triggerd = false;

    private void Start()
    {
        CreatedItem = this.gameObject;
        childCamera = Camera.main.GetComponent<CameraRotateVertically>();
        cameraZoom = GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<CameraZoom>();
        cameraZoom.currentItem = this.gameObject;
        parentCamera = GameObject.FindGameObjectWithTag("CameraHolder").GetComponent<CameraRotateHorizontaly>();
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        FindSlot();
        SlipperyOrNot();
    }

    private void Update()
    {
        FindLastItem();
        if (isItemDropped == true)
        {
            CanControllItem = false;
            cameraZoom.currentItem = null;
        }

        if (isItemOnTopOfSurface && CanControllItem)
        {
            //Rotate Item If On TopSurface
            transform.Rotate(0f, 0f, 1f);
        }
        foreach (Touch touch in Input.touches)
        {
            //Debug.Log(Input.mousePosition.y);
            HandleTouchInputs();
        }
    }
    public void SlipperyOrNot()
    {
        if (levelParameters.DoesLevelIncludeSlipperyPart)
        {
            if (!levelParameters.SlipperButtonActive)
            {
                if (GetComponent<BoxCollider>() != null)
                {
                    GetComponent<BoxCollider>().material = null;
                }
                if (GetComponent<MeshCollider>() != null)
                {
                    GetComponent<MeshCollider>().material = null;
                }
            }
        }
    }
    public void FindLastItem()
    {
        for(int i = 0; i < levelParameters.itemSlots.Length; i++)
        {
            if (CreatedItem.name == levelParameters.itemPrefab[i].name)
            {
                if (itemSlot.numberOfThisItem == 0)
                    isLastItem = true;
            }
        }
    }
    void FindSlot()
    {
        for (int i = 0; i < levelParameters.itemSlots.Length; i++)
        {
            if (CreatedItem.name == levelParameters.itemPrefab[i].name)
            {
                itemSlot = levelParameters.itemSlots[i];
            }
        }
    }
    private void HandleTouchInputs()
    {
        if (Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            if (CanControllItem)
            {
                eventDataCurrentPosition = new PointerEventData(EventSystem.current);  //Detect RayCast On ItemSlots
                eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                ToggleReturnItem(results[0].gameObject);   //End Of Detect RayCast On ItemSlots
                if(GetComponent<BoxCollider>() != null)
                    GetComponent<BoxCollider>().enabled = false;
                if (GetComponent<SphereCollider>() != null)
                    GetComponent<SphereCollider>().enabled = false;
                childCamera.canRotateCameraVertically = false;
                parentCamera.canRotateCameraHorizontaly = false;
 
                touchPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                //improvedTouchPos = new Vector3(touchPos.x, touchPos.y + holdItemHigher, touchPos.z);

                //Normal Vision Top Surface
                if (isItemOnTopOfSurface && !childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
                {
                    if (transform.localPosition.y > 5f)
                    {
                        touchPos.y = 6.25f;
                    }
                    /*if (childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
                    {
                        if (!isItemOnTopOfSurface)
                            improvedTouchPos = new Vector3(touchPos.x + 0.5f,
                                touchPos.y + 0f, touchPos.z + 0.5f);
                        if (isItemOnTopOfSurface)
                            
                    }*/
                    improvedTouchPos = new Vector3(touchPos.x + HandleTopSurfaceXBehaviour(),
                        touchPos.y + holdItemHigher, touchPos.z + HandleTopSurfaceZBehaviour());
                    LockPosOutOfBounds();
                }
                //God Vision
                else if (childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
                {
                    if (parentCamera.transform.localEulerAngles.y / 180 < 1)
                    {
                        // 0 - 20 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) < 1)
                            improvedTouchPos = new Vector3(touchPos.x - 2f, touchPos.y - 0f, touchPos.z - 2f);

                        // 20 - 40 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 1 && (parentCamera.transform.localEulerAngles.y / 20f) < 2)
                            improvedTouchPos = new Vector3(touchPos.x - 2.5f, touchPos.y - 0f, touchPos.z - 1f);

                        // 40 - 60 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 2 && (parentCamera.transform.localEulerAngles.y / 20f) < 3)
                            improvedTouchPos = new Vector3(touchPos.x - 2.2f, touchPos.y - 0f, touchPos.z - 0.5f);

                        // 60 - 80 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 3 && (parentCamera.transform.localEulerAngles.y / 20f) < 4)
                            improvedTouchPos = new Vector3(touchPos.x - 2.4f, touchPos.y - 0f, touchPos.z + 0.25f);

                        // 80 - 100 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 4 && (parentCamera.transform.localEulerAngles.y / 20f) < 5)
                            improvedTouchPos = new Vector3(touchPos.x - 2.3f, touchPos.y - 0f, touchPos.z + 0.75f);

                        // 100 - 120 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 5 && (parentCamera.transform.localEulerAngles.y / 20f) < 6)
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z + 2.2f);

                        // 120 - 140 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 6 && (parentCamera.transform.localEulerAngles.y / 20f) < 7)
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z + 2.5f);

                        // 140 - 160 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 7 && (parentCamera.transform.localEulerAngles.y / 20f) < 8)
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z + 2.8f);

                        // 160 - 179 Degree
                        if ((parentCamera.transform.localEulerAngles.y / 20f) > 8 && (parentCamera.transform.localEulerAngles.y / 20f) < 9)
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z + 1.75f);
                    }

                    if (parentCamera.transform.localEulerAngles.y / 180 >= 1)
                    {
                        // 0 to -20 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360)/ -20f < 1)
                            improvedTouchPos = new Vector3(touchPos.x - 1f, touchPos.y - 0f, touchPos.z - 2f);

                        // -20 to -40 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 1 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 2))
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z - 2f);

                        // -40 to -60 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 2 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 3))
                            improvedTouchPos = new Vector3(touchPos.x - 0f, touchPos.y - 0f, touchPos.z - 2f);

                        // -60 to -80 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 3 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 4))
                            improvedTouchPos = new Vector3(touchPos.x + 1.5f, touchPos.y - 0f, touchPos.z - 1.5f);

                        // -80 to -100 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 4 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 5))
                            improvedTouchPos = new Vector3(touchPos.x + 1.75f, touchPos.y - 0f, touchPos.z - 0.5f);

                        // -100 to -120 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 5 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 6))
                            improvedTouchPos = new Vector3(touchPos.x + 1.5f, touchPos.y - 0f, touchPos.z - 0.5f);

                        // -120 to -140 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 6 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 7))
                            improvedTouchPos = new Vector3(touchPos.x + 1.5f, touchPos.y - 0f, touchPos.z - 0.5f);

                        // -140 to -160 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 7 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 8))
                            improvedTouchPos = new Vector3(touchPos.x + 2f, touchPos.y - 0f, touchPos.z - 0.2f);

                        // -160 to -179 Degree
                        if ((parentCamera.transform.localEulerAngles.y - 360) / -20f > 8 && ((parentCamera.transform.localEulerAngles.y - 360) / -20f < 9))
                            improvedTouchPos = new Vector3(touchPos.x +2f, touchPos.y - 0f, touchPos.z + 0.5f);
                    }
                }
                //Normal Vision BelowSurface
                if (!isItemOnTopOfSurface && !childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition) 
                {
                    improvedTouchPos = new Vector3(touchPos.x + 0f,
                        touchPos.y +holdItemHigherBelowSurface, touchPos.z + 0f);
                }
                if (!isOutOfBounds)
                    CreatedItem.transform.localPosition = improvedTouchPos;
            }
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            if (eventDataCurrentPosition != null && results != null)
            {
                ReturnItem(results[0].gameObject);
            }
            if (isLastItem)
            {
                itemSlot.LastItemDropped = true;
            }
            CanControllItem = false;
            if (GetComponent<BoxCollider>() != null)
                GetComponent<BoxCollider>().enabled = true;
            if (GetComponent<SphereCollider>() != null)
                GetComponent<SphereCollider>().enabled = true;
            CreatedItem.GetComponent<Rigidbody>().isKinematic = false;
            childCamera.canRotateCameraVertically = true;
            parentCamera.canRotateCameraHorizontaly = true;
        }
    }
    void LockPosOutOfBounds()
    {
        float lockPoint = FindLockPoint();
        if (!childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
        {
            if (Input.GetTouch(0).position.y > lockPoint)
            {
                isOutOfBounds = true;
            }
            if (Input.GetTouch(0).position.y < lockPoint)
            {
                isOutOfBounds = false;
            }
        }
        if (childCamera.GetComponent<CameraRotateVertically>().isInGodVisionPosition)
        {
            if (Input.GetTouch(0).position.y > lockPoint + 200f)
            {
                isOutOfBounds = true;
            }
            if (Input.GetTouch(0).position.y < lockPoint + 200f)
            {
                isOutOfBounds = false;
            }
        }
    }
    float FindLockPoint()
    {
        float lockPointToReturn = 1200f;
        switch (Screen.height)
        {
            case 800:
                lockPointToReturn = 500f;
                upxChangeFactor = 0.6f;
                upzChangeFactor = 0.6f;
                break;
            case 1280:
                lockPointToReturn = 700f;
                upxChangeFactor = 1f;
                upzChangeFactor = 1f;
                break;
            case 1920:
                lockPointToReturn = 1000f;
                upxChangeFactor = 1.1f;
                upzChangeFactor = 1.1f;
                break;
            case 2160:
                lockPointToReturn = 1150f;
                upxChangeFactor = 0.55f;
                upzChangeFactor = 0.55f;
                break;
            case 2340:
                lockPointToReturn = 1200f;
                upxChangeFactor = 0.65f;
                upzChangeFactor = 0.65f;
                break;
            case 2400:
                lockPointToReturn = 1300f;
                break;
            case 2560:
                lockPointToReturn = 1370f;
                upxChangeFactor = 0.3f;
                upzChangeFactor = 0.3f;
                break;
            case 2960:
                lockPointToReturn = 1600f;
                upxChangeFactor = 0.25f;
                upzChangeFactor = 0.25f;
                break;
        }
        return lockPointToReturn;
    }
    float HandleTopSurfaceXBehaviour()
    {
        float valueToReturn = 0f;
        float midPoint = FindMidPoint();
        ChangeXSmoothnessValues();
        if (Input.GetTouch(0).position.y > midPoint) 
        {
            float howMuchTen = ((Input.GetTouch(0).position.y - midPoint) / 10);
            valueToReturn = -UPxsmooth * howMuchTen * upxChangeFactor;
        }
        if (Input.GetTouch(0).position.y < midPoint)
        {
            float howMuchTen = ((midPoint - Input.GetTouch(0).position.y ) / 10);
            valueToReturn = DOWNxsmooth * howMuchTen * downChangeFactor;
        }
        return valueToReturn;
    }

    void ChangeXSmoothnessValues()
    {
        bool isNegative = false;
        int howManyTenDegrees = 0;
        if(parentCamera.transform.localEulerAngles.y / 180 < 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) > 0)
                howManyTenDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y) / 10; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) < 0)
            {
                howManyTenDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -10; //Negative Degrees
                isNegative = true;
            }
        }
        if (parentCamera.transform.localEulerAngles.y / 180 >= 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y -360)) > 0)
                howManyTenDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y - 360) / 10; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) < 0)
            {
                howManyTenDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -10; //Negative Degrees
                isNegative = true;
            }
        }
        int howManyNintyDegrees = 0;
        if (parentCamera.transform.localEulerAngles.y / 180 < 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) > 0)
                howManyNintyDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y) / 90; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) < 0)
            {
                howManyNintyDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -90; ///negative Degreess
                isNegative = true;
            }
        }
        if (parentCamera.transform.localEulerAngles.y / 180 >= 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) > 0)
                howManyNintyDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y - 360) / 90; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y -360)) < 0)
            {
                howManyNintyDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -90; ///negative Degreess
                isNegative = true;
            }
        }
        if (howManyNintyDegrees > 0 && howManyTenDegrees != 9) //After 90 Dgerees Are The SAME As 0 - 90 Degrees 
        {
            howManyTenDegrees -= 10;
        }
        //Debug.Log(howManyTenDegrees);
        switch (howManyTenDegrees)
        {
            //parent Camera Rotated 0 , 100 , -100 Degree  - Checked
            case 0:
                UPxsmooth = 0.35f;  //0 Dgree
                DOWNxsmooth = 0.1f;
                if (howManyNintyDegrees > 0) //100 Degree
                {
                    UPxsmooth = 0.25f;
                    DOWNxsmooth = 0.1f;
                    if (isNegative) //-100 Degree
                    {
                        UPxsmooth = -0.5f;
                        DOWNxsmooth = -0.15f;
                    }

                }
                break;
            //parent Camera Rotated 10 , 110 , -10 , -110 Degree  - Checked
            case 1:
                UPxsmooth = 0.3f; //10 Degree
                DOWNxsmooth = 0.1f;
                if (isNegative)
                {
                    UPxsmooth = 0.3f; //-10 Degree
                    DOWNxsmooth = 0.15f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = 0.25f; //110 Degree
                    DOWNxsmooth = 0.05f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.45f; //-110 Degree
                        DOWNxsmooth = -0.1f;
                    }
                }
                break;
            //parent Camera Rotated 20 , 120 , -20 , -120 Degree - Checked
            case 2:
                UPxsmooth = 0.4f; //20 Degree
                DOWNxsmooth = 0.1f;
                if (isNegative)
                {
                    UPxsmooth = 0.2f; //-20 Degree
                    DOWNxsmooth = 0.1f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = 0.05f; //120 Degree
                    DOWNxsmooth = 0.1f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.45f; //-120 Degree
                        DOWNxsmooth = -0.15f;
                    }
                }
                break;
            //parent Camera Rotated 30 ,130 , -30 , -130 Degree - Checked
            case 3:
                UPxsmooth = 0.4f; //30 Degree
                DOWNxsmooth = 0.15f;
                if (isNegative)
                {
                    UPxsmooth = 0.1f; //-30 Degree
                    DOWNxsmooth = 0.1f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = 0.1f; //130 Degree
                    DOWNxsmooth = 0.1f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.5f; //-130 Degree
                        DOWNxsmooth = -0.15f;
                    }
                }
                break;
            //parent Camera Rotated 40 , 140 , -40 , -140 Degree - Checked
            case 4:
                UPxsmooth = 0.45f; // 40 Degree
                DOWNxsmooth = 0.15f;
                if (isNegative)
                {
                    UPxsmooth = 0.1f; // -40 Degree
                    DOWNxsmooth = 0.05f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = 0.1f;// 140 Degree
                    DOWNxsmooth = 0.05f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.5f;// -140 Degree
                        DOWNxsmooth = -0.15f;
                    }
                }
                break;
            //parent Camera Rotated 50 , 150 , -50 , -150 Degree - Checked
            case 5:
                UPxsmooth = 0.5f; //50 Degree
                DOWNxsmooth = 0.15f;
                if (isNegative)
                {
                    UPxsmooth = -0.1f; //-50 Degree
                    DOWNxsmooth = -0.05f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = 0.005f; //150 Degree
                    DOWNxsmooth = 0.005f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.5f; //-150 Degree
                        DOWNxsmooth = -0.15f;
                    }
                }
                break;
            //parent Camera Rotated 60 , 160 , -60 , -160 Degree - Checked
            case 6:
                UPxsmooth = 0.5f; //60 Degree
                DOWNxsmooth = 0.2f;
                if (isNegative)
                {
                    UPxsmooth = -0.25f; //-60 Degree
                    DOWNxsmooth = -0.1f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = -0.1f; //160 Degree
                    DOWNxsmooth = -0.005f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.4f; //-160 Degree
                        DOWNxsmooth = -0.15f;
                    }
                }
                break;
            //parent Camera Rotated 70 , 170 , -70 , -170 Degree - Checked
            case 7:
                UPxsmooth = 0.45f; //70 Degree
                DOWNxsmooth = 0.15f;
                if (isNegative)
                {
                    UPxsmooth = -0.25f; //-70 Degree
                    DOWNxsmooth = -0.1f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPxsmooth = -0.3f; //170 Degree
                    DOWNxsmooth = -0.05f;
                    if (isNegative)
                    {
                        UPxsmooth = -0.45f; //-170 Degree
                        DOWNxsmooth = -0.1f;
                    }
                }
                break;
            //parent Camera Rotated 80 , -80 Degree - Checked
            case 8:
                UPxsmooth = 0.35f; //80 Degree
                DOWNxsmooth = 0.1f;
                if (isNegative)
                {
                    UPxsmooth = -0.3f; //-80 Degree
                    DOWNxsmooth = -0.1f;
                }
                break;
            //parent Camera Rotated 90, -90 Degree - Checked
            case 9:
                UPxsmooth = 0.3f; //90 Degree
                DOWNxsmooth = 0.1f;
                if (isNegative)
                {
                    UPxsmooth = -0.3f; //-90 Degree
                    DOWNxsmooth = -0.1f;
                }
                break;
        }
    }

    float HandleTopSurfaceZBehaviour()
    {
        float valueToReturn = 0;
        float midPoint = FindMidPoint();
        ChangeZSmoothnessValues();
        if (Input.GetTouch(0).position.y > midPoint)
        {
            float howMuchTen = ((Input.GetTouch(0).position.y - midPoint) / 10);
            valueToReturn = -UPzsmooth * howMuchTen * upzChangeFactor;
        }
        if (Input.GetTouch(0).position.y < midPoint)
        {
            float howMuchTen = ((midPoint - Input.GetTouch(0).position.y) / 10);
            valueToReturn = DOWNzsmooth * howMuchTen * downChangeFactor;
        }
        return valueToReturn;
    }
    void ChangeZSmoothnessValues()
    {
        bool isNegative = false;
        int howManyTenDegrees = 0;
        if(parentCamera.transform.localEulerAngles.y / 180 < 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) > 0)
               howManyTenDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y) / 10; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) < 0)
            {
                howManyTenDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -10; //Negative Degrees
                isNegative = true;
            }
        }
        if (parentCamera.transform.localEulerAngles.y / 180 >= 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) > 0)
                howManyTenDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y - 360) / 10; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) < 0)
            {
                howManyTenDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -10; //Negative Degrees
                isNegative = true;
            }
        }
        int howManyNintyDegrees = 0;
        if(parentCamera.transform.localEulerAngles.y /180 < 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) > 0)
                howManyNintyDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y) / 90; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y)) < 0)
            {
                howManyNintyDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -90; ///negative Degreess
                isNegative = true;
            }
        }
        if (parentCamera.transform.localEulerAngles.y /180 >= 1)
        {
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) > 0)
                howManyNintyDegrees = (int)Mathf.Abs(parentCamera.transform.localEulerAngles.y - 360) / 90; //Positive Degrees
            if ((Mathf.Sign(parentCamera.transform.localEulerAngles.y - 360)) < 0)
            {
                howManyNintyDegrees = (int)(parentCamera.transform.localEulerAngles.y - 360) / -90; ///negative Degreess
                isNegative = true;
            }
        }
        if (howManyNintyDegrees > 0 && howManyTenDegrees != 9) //After 90 Dgerees Are The SAME As 0 - 90 Degrees 
        {
            howManyTenDegrees -= 10;
        }
        switch (howManyTenDegrees)
        {
            //parent Camera Rotated 0 , 100 , -100 Degree - Checked
            case 0:
                UPzsmooth = 0.3f; //0 Degree
                DOWNzsmooth = 0.15f;
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.45f; //100 Degree 
                    DOWNzsmooth = -0.05f;
                    if (isNegative)
                    {
                        UPzsmooth = 0.15f; //-100 Degree 
                        DOWNzsmooth = 0.05f;
                    }
                }
                break;
            //parent Camera Rotated 10 , 110 , -10 , -110 Degree - Checked
            case 1:
                UPzsmooth = 0.4f; //10 Degree
                DOWNzsmooth = 0.15f;
                if (isNegative)
                {
                    UPzsmooth = 0.35f; //-10 Degree
                    DOWNzsmooth = 0.15f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.45f; //110 Degree
                    DOWNzsmooth = -0.2f;
                    if (isNegative)
                    {
                        UPzsmooth = 0.15f; //-110 Degree
                        DOWNzsmooth = 0.02f;
                    }
                }
                break;
            //parent Camera Rotated 20 , 120 , -20 , -120 Degree - Checked
            case 2:
                UPzsmooth = 0.3f; //20 Degree
                DOWNzsmooth = 0.03f;
                if (isNegative)
                {
                    UPzsmooth = 0.45f; //-20 Degree
                    DOWNzsmooth = 0.2f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.5f; //120 Degree
                    DOWNzsmooth = -0.15f;
                    if (isNegative)
                    {
                        UPzsmooth = 0.1f; //-120 Degree
                        DOWNzsmooth = -0.01f;
                    }
                }
                break;
            //parent Camera Rotated 30 , 130 , -30 , -130 Degree - Checked
            case 3:
                UPzsmooth = 0.2f; //30 Degree
                DOWNzsmooth = 0.05f;
                if (isNegative)
                {
                    UPzsmooth = 0.45f; //-30 Degree
                    DOWNzsmooth = 0.2f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.45f; //130 Degree
                    DOWNzsmooth = -0.1f;
                    if (isNegative)
                    {
                        UPzsmooth = -0.05f; //-130 Degree
                        DOWNzsmooth = -0.05f;
                    }
                }
                break;
            //parent Camera Rotated 40 , 140 , -40 , -140 Degree - Checked
            case 4:
                UPzsmooth = 0.2f; //40 Degree
                DOWNzsmooth = 0.05f;
                if (isNegative)
                {
                    UPzsmooth = 0.5f; //-40 Degree
                    DOWNzsmooth = 0.25f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.45f; //140 Degree
                    DOWNzsmooth = -0.15f;
                    if (isNegative)
                    {
                        UPzsmooth = -0.1f; //-140 Degree
                        DOWNzsmooth = -0.05f;
                    }
                }
                break;
            //parent Camera Rotated 50 , 150 , -50 , -150 Degree - Checked
            case 5:
                UPzsmooth = 0.1f; //50 Degree
                DOWNzsmooth = 0.1f;
                if (isNegative)
                {
                    UPzsmooth = 0.45f; //-50 Degree
                    DOWNzsmooth = 0.15f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.5f; //150 Degree
                    DOWNzsmooth = -0.15f;
                    if (isNegative)
                    {
                        UPzsmooth = -0.1f; //-150 Degree
                        DOWNzsmooth = -0.05f;
                    }
                }
                break;
            //parent Camera Rotated 60 , 160 , -60 , -160 Degree - Checked
            case 6:
                UPzsmooth = 0.05f; //60 Degree
                DOWNzsmooth = 0.05f;
                if (isNegative)
                {
                    UPzsmooth = 0.45f; //-60 Degree
                    DOWNzsmooth = 0.25f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.45f; //160 Degree
                    DOWNzsmooth = -0.15f;
                    if (isNegative)
                    {
                        UPzsmooth = -0.3f; //-160 Degree
                        DOWNzsmooth = -0.07f;
                    }
                }
                break;
            //parent Camera Rotated 70 , 170 , -70 , -170 Degree - Checked
            case 7:
                UPzsmooth = -0.2f; //70 Degree
                DOWNzsmooth = -0.05f;
                if (isNegative)
                {
                    UPzsmooth = 0.45f; //-70 Degree
                    DOWNzsmooth = 0.1f;
                }
                if (howManyNintyDegrees > 0)
                {
                    UPzsmooth = -0.4f; //170 Degree
                    DOWNzsmooth = -0.15f;
                    if (isNegative)
                    {
                        UPzsmooth = -0.25f; //-170 Degree
                        DOWNzsmooth = -0.05f;
                    }
                }
                break;
            //parent Camera Rotated 80 , -80 Degree - Checked
            case 8:
                UPzsmooth = -0.3f; //80 Degree
                DOWNzsmooth = -0.1f;
                if (isNegative)
                {
                    UPzsmooth = 0.35f; //-80 Degree
                    DOWNzsmooth = 0.05f;
                }
                break;
            //parent Camera Rotated 90 , -90 Degree - Checked
            case 9:
                UPzsmooth = -0.4f; //90 Degree
                DOWNzsmooth = -0.1f;
                if (isNegative)
                {
                    UPzsmooth = 0.35f; //-90 Degree
                    DOWNzsmooth = 0.1f;
                }
                break;
        }
    }
    float FindMidPoint()
    {
        float MidPointToReturn = 950f;
        switch (Screen.height)
        {
            case 800:
                MidPointToReturn = 350f;
                break;
            case 1280:
                MidPointToReturn = 600f;
                break;
            case 1920:
                MidPointToReturn = 900f;
                break;
            case 2160:
                MidPointToReturn = 950f;
                break;
            case 2340:
                MidPointToReturn = 1050f;
                break;
            case 2400:
                MidPointToReturn = 1050f;
                break;
            case 2560:
                MidPointToReturn = 1050f;
                break;
            case 2960:
                MidPointToReturn = 1300f;
                break;
        }
        //Debug.Log(MidPointToReturn);
        return MidPointToReturn;
    }
    void ToggleReturnItem(GameObject slot)
    {
        if (slot.name == "Toggle ReturnToSlot")
        {
            canReturnToSlot = true;
        }
    }
    void ReturnItem(GameObject slot)
    {
        if(slot.GetComponent<ItemSlot>() != null)
        {
            if (slot.GetComponent<ItemSlot>().ItemPrefab.name == CreatedItem.name)
            {
                if (canReturnToSlot) 
                {
                    levelParameters.IncreaseNumberOfItem(gameObject.name);
                    itemSlot.LastItemDropped = false;
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        //collisions.Add(other.gameObject);
        if (other.gameObject.tag == "Surface")
        {
            if(other.gameObject.GetComponent<ColourfulDiskPart>() == null)
            {
                if (!itemTouchedAnotherItem && !itemTouchedDisk)
                    levelParameters.numberOfAllItems--;
                itemTouchedDisk = true;
                CanControllItem = false;
            }
            if(dropVFX != null)
                dropVFX.SetActive(true);

        }
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Blue" || other.gameObject.tag == "Red" ||
            other.gameObject.tag == "Green" || other.gameObject.tag == "Yellow" || other.gameObject.tag == "Food"
            || other.gameObject.tag == "Fruit")
        {
            collisions.Add(other.gameObject);
            if (!itemTouchedDisk && collisions.Count <= 1)
                levelParameters.numberOfAllItems--;
            CanControllItem = false;
            itemTouchedAnotherItem = true;
        }
        if (other.gameObject.tag == "Ground")
        {
            levelParameters.IncreaseNumberOfItem(gameObject.name);
            if (itemTouchedDisk || itemTouchedAnotherItem)
                levelParameters.numberOfAllItems++;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ColourfulDiskPart>() != null)
        {
            other.gameObject.GetComponent<ColourfulDiskPart>().howManyColorfulDisksCollidedWith = collisions.Count;
        }
    }
}
