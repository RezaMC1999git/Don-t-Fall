using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPartDisk : MonoBehaviour
{
    LevelParameters levelParameters;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemBehaviour>() != null)
        {
            if (other.GetComponent<ItemBehaviour>().triggerd == false)
            {
                if (other.gameObject.tag == "Fruit")
                {
                    levelParameters = other.gameObject.GetComponent<ItemBehaviour>().levelParameters;
                    if (other.gameObject.tag != this.gameObject.tag)
                    {
                        levelParameters.IncreaseNumberOfItem(other.gameObject.name);
                        levelParameters.numberOfAllItems++;
                        other.GetComponent<ItemBehaviour>().triggerd = true;
                        Destroy(other.gameObject);
                    }
                }
            }
        }
    }
}
