using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyDisk : MonoBehaviour
{
    public PhysicMaterial slipperyMaterial;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<ItemBehaviour>() != null)
        {
            if ((other.gameObject.CompareTag("Item") || other.gameObject.CompareTag("Red") || other.gameObject.CompareTag("Blue")
                || other.gameObject.CompareTag("Green") || other.gameObject.CompareTag("Yellow"))
                    && !other.gameObject.GetComponent<ItemBehaviour>().CanControllItem)
            {
                if (other.gameObject.GetComponent<MeshCollider>() != null)
                    other.gameObject.GetComponent<MeshCollider>().material = slipperyMaterial;
                if (other.gameObject.GetComponent<BoxCollider>() != null)
                    other.gameObject.GetComponent<BoxCollider>().material = slipperyMaterial;
            }
        }
    }
}
