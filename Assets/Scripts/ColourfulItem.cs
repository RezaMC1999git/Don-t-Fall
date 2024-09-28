using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourfulItem : MonoBehaviour
{
    LevelParameters levelParameters;
    public bool needToBeDestroyed = false;

    private void Update()
    {
        if (needToBeDestroyed)
            Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<ItemBehaviour>() != null)
        {
            levelParameters = other.gameObject.GetComponent<ItemBehaviour>().levelParameters;
            if (other.gameObject.tag != this.gameObject.tag)
            {
                if (other.gameObject.GetComponent<ColourfulItem>() != null)
                {
                    if(gameObject.GetComponent<ItemBehaviour>().itemTouchedDisk) //This Is On Disk Item
                    {
                        needToBeDestroyed = false;
                        other.gameObject.GetComponent<ColourfulItem>().needToBeDestroyed = true;
                        levelParameters.IncreaseNumberOfItem(other.gameObject.name);
                        levelParameters.numberOfAllItems++;
                        other.gameObject.GetComponent<ItemBehaviour>().triggerd = true;
                    }
                    else if(other.gameObject.GetComponent<ItemBehaviour>().itemTouchedDisk) //The Other Item Is On Yhe Disk
                    {
                        needToBeDestroyed = true;
                        other.gameObject.GetComponent<ColourfulItem>().needToBeDestroyed = false;
                    }
                }
            }
        }
    }
}
