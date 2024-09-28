using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelParameters : MonoBehaviour
{
    public static LevelParameters instacne;
    public int numberOfAllItems = 0;
    int firstCaclulatedNumberOfItems = 0;
    public int[] numberOfItem;
    public GameObject[] itemPrefab;
    public GameObject WinPanel;
    public TextMeshProUGUI countDownText;
    public ItemSlot[] itemSlots;
    public SlipperOffTrigger slipperyTriggerOff;
    public bool DoesLevelIncludeSlipperyPart = false;
    [Tooltip("True If DoesLevelIncludeSlipperyPart is True")] public bool SlipperButtonActive = false;
    [SerializeField] bool startWinCoroutineOnce = true;
    public bool isLevelInProgress = false;
    int currentLevelIndex;

    private void Start()
    {
        if (DoesLevelIncludeSlipperyPart)
        {
            //slippeOffTrigger.CheckIfNeedToBeActive();
        }
        if (instacne == null)
            instacne = this;
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        foreach(int nums in numberOfItem)
        {
            numberOfAllItems += nums;
        }
        firstCaclulatedNumberOfItems = numberOfAllItems;
    }
    private void Update()
    {
        if (numberOfAllItems <= 0 && startWinCoroutineOnce)
        {
            startWinCoroutineOnce = false;
            if(isLevelInProgress)
                StartCoroutine(WinLevel());
        }
    }
    public int numberOfThisItem(int number)
    {
        int numToReturn = 0;
        switch (number)
        {
            case 1:
                numToReturn = numberOfItem[0];
                break;
            case 2:
                numToReturn = numberOfItem[1];
                break;
            case 3:
                numToReturn = numberOfItem[2];
                break;
            case 4:
                numToReturn = numberOfItem[3];
                break;
            case 5:
                numToReturn = numberOfItem[4];
                break;
            case 6:
                numToReturn = numberOfItem[5];
                break;
            case 7:
                numToReturn = numberOfItem[6];
                break;
            case 8:
                numToReturn = numberOfItem[7];
                break;
        }
        return numToReturn;
    }

    public GameObject ItemPrefab(int number)
    {
        GameObject itemToReturn = null;
        switch (number)
        {
            case 1:
                itemToReturn = itemPrefab[0];
                break;
            case 2:
                itemToReturn = itemPrefab[1];
                break;
            case 3:
                itemToReturn = itemPrefab[2];
                break;
            case 4:
                itemToReturn = itemPrefab[3];
                break;
            case 5:
                itemToReturn = itemPrefab[4];
                break;
            case 6:
                itemToReturn = itemPrefab[5];
                break;
            case 7:
                itemToReturn = itemPrefab[6];
                break;
            case 8:
                itemToReturn = itemPrefab[7];
                break;
        }
        return itemToReturn;
    }

    public void ReduceNumberOfItem(string itemName)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if (itemName == itemPrefab[i].name)
            {
                itemSlots[i].GetComponent<ItemSlot>().CreateItem();
            }
        }
    }
    public void IncreaseNumberOfItem(string itemName)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemName == itemPrefab[i].name)
            {
                itemSlots[i].GetComponent<ItemSlot>().ReGenerateItem();
            }
        }
    }
    
    IEnumerator WinLevel()
    {
        if (numberOfAllItems == 0 && isLevelInProgress)
            countDownText.text = "4";
        countDownText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        if (numberOfAllItems == 0 && isLevelInProgress)
            countDownText.text = "3";
        else
        {
            countDownText.gameObject.SetActive(false);
            startWinCoroutineOnce = true;
            yield break;
        }
        yield return new WaitForSeconds(1f);

        if (numberOfAllItems == 0 && isLevelInProgress)
            countDownText.text = "2";
        else
        {
            countDownText.gameObject.SetActive(false);
            startWinCoroutineOnce = true;
            yield break;
        }
        yield return new WaitForSeconds(1f);

        if (numberOfAllItems == 0 && isLevelInProgress)
            countDownText.text = "1";
        else
        {
            countDownText.gameObject.SetActive(false);
            startWinCoroutineOnce = true;
            yield break;
        }
        yield return new WaitForSeconds(1f);

        if (numberOfAllItems == 0 && isLevelInProgress)
        {
            countDownText.gameObject.SetActive(false);
            if (isLevelInProgress)
            {
                FindObjectOfType<GameController>().itemSlotsCanvas.SetActive(false);
                Time.timeScale = 0f;
                WinPanel.SetActive(true);
            }
            //if (!isLevelInProgress)
            //    yield break;
        }
        else
        {
            countDownText.gameObject.SetActive(false);
            startWinCoroutineOnce = true;
            yield break;
        }
    }
    public void callculatenumberOfAllItems()
    {
        if (numberOfAllItems != firstCaclulatedNumberOfItems)
        {
            numberOfAllItems = 0;
            foreach (int nums in numberOfItem)
            {
                numberOfAllItems += nums;
            }
        }
        startWinCoroutineOnce = true;
    }
    public void ResetSlipperyMaterials()
    {
        if (DoesLevelIncludeSlipperyPart)
        {
            slipperyTriggerOff.InitialSlipperyDisk();
        }
    }
}
