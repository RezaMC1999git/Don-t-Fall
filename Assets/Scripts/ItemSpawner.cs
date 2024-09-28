using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner instance;

    Transform itemsParent;
    [SerializeField] GameObject CreatedItem;
    [SerializeField] LevelParameters levelParameters;

    public bool canReturnToSlot = false;
    //[SerializeField] float createHigherFactor = 10f;
    [Tooltip("Child Camera")] [SerializeField] Camera mainCamera;
    private void Start()
    {
        if (instance == null)
            instance = this;
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        itemsParent = GameObject.FindGameObjectWithTag("ItemsParent").transform;
    }
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            int id = touch.fingerId;
            if (EventSystem.current.IsPointerOverGameObject(id))
            {
                TouchedToCreateItem();
            }
        }
    }

    private void TouchedToCreateItem()
    {
        levelParameters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Detect What Item Are We Touching
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            //Instantiate Item
            if (results[0].gameObject.GetComponent<ItemSlot>() != null)
            {
                if (mainCamera.transform.localPosition.y >= 0)
                {
                    var currentItem = results[0].gameObject.GetComponent<ItemSlot>().ItemPrefab;
                    Vector3 touchPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                    CreatedItem = Instantiate(currentItem, touchPos, Quaternion.Euler(-90f,0f,0f));
                    CreatedItem.SetActive(true);
                    CreatedItem.name = currentItem.name;
                    //CreatedItem.GetComponent<ItemBehaviour>().FindLastItem();
                    CreatedItem.transform.parent = itemsParent;
                    levelParameters.ReduceNumberOfItem(CreatedItem.name);
                    CreatedItem.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
    }

}

