using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public static ItemSlot instance;
    public LevelParameters levelParamaeters;
    public int numberOfSlot;
    public int numberOfThisItem;
    public GameObject ItemPrefab;
    public bool LastItemDropped = false;

    [Header("UI Elements")]
    [SerializeField] Image itemPreview;
    [SerializeField] TextMeshProUGUI wightText;
    [SerializeField] TextMeshProUGUI itemsNumberText;

    float itemWeight;
    Rigidbody rigidBody;
    public void Initialize()
    {
        levelParamaeters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        ItemPrefab = levelParamaeters.ItemPrefab(numberOfSlot);
        numberOfThisItem = levelParamaeters.numberOfThisItem(numberOfSlot);
        if (numberOfThisItem > 0)
            gameObject.SetActive(true);
        itemWeight = ItemPrefab.GetComponent<Rigidbody>().mass;
        itemPreview.sprite = ItemPrefab.GetComponent<ItemBehaviour>().itemPreviewSprite;
        wightText.text = (itemWeight * 100f) + " gr".ToString();
        itemsNumberText.text = (numberOfThisItem + " X").ToString();
    }
    private void Start()
    {
        if (instance == null)
            instance = this;
        levelParamaeters = FindCurrentLevelParametes.instance.ConnectLevelParametersToItemSlot();
        ItemPrefab = levelParamaeters.ItemPrefab(numberOfSlot);
        numberOfThisItem = levelParamaeters.numberOfThisItem(numberOfSlot);
        itemWeight = ItemPrefab.GetComponent<Rigidbody>().mass;
        itemPreview.sprite = ItemPrefab.GetComponent<ItemBehaviour>().itemPreviewSprite;
        wightText.text = (itemWeight * 100f) + " gr".ToString();
        itemsNumberText.text = (numberOfThisItem + " X").ToString();
    }
    private void Update()
    {
        if (numberOfThisItem == 0)
        {
            if (LastItemDropped)
                gameObject.SetActive(false);
        }
        if (numberOfThisItem == 1)
            LastItemDropped = false;
        if (numberOfThisItem < 0)
            gameObject.SetActive(false);
    }
    public void CreateItem()
    {
        numberOfThisItem--;
        itemsNumberText.text = (numberOfThisItem + " X").ToString();
    }
    public void ReGenerateItem()
    {
        numberOfThisItem++;
        itemsNumberText.text = (numberOfThisItem + " X").ToString();
        if (numberOfThisItem >= 0)
        {
            gameObject.SetActive(true);
            LastItemDropped = false;
        }
    }
}
