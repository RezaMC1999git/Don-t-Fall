using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviourCollider : MonoBehaviour
{
    //bool collidedWithItem = false;
    public List<GameObject> triggers = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Red" || other.gameObject.tag == "Green" ||
            other.gameObject.tag == "Blue" || other.gameObject.tag == "Yellow")
        {
            //collidedWithItem = true;
            GetComponentInParent<ItemBehaviour>().holdItemHigher += 0.5f;
            triggers.Add(other.gameObject);
        }
        if (other.gameObject.CompareTag("Surface"))
        {
            GetComponentInParent<ItemBehaviour>().holdItemHigher += 0.4f;
        }
        if (other.gameObject.CompareTag("TopSurfaceCollider"))
        {
            GetComponentInParent<ItemBehaviour>().isItemOnTopOfSurface = true;
            //GetComponentInParent<ItemBehaviour>().holdItemHigher += 0.3f;
        }
        if (other.gameObject.CompareTag("Item") || other.gameObject.CompareTag("Fruit") || other.gameObject.CompareTag("Food"))
        {
            //collidedWithItem = true;
            GetComponentInParent<ItemBehaviour>().holdItemHigher += 0.5f;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            //GetComponentInParent<ItemBehaviour>().holdItemHigher -= 0.5f;
        }
        if (other.gameObject.CompareTag("Surface"))
        {
            //GetComponentInParent<ItemBehaviour>().holdItemHigher -= 0.4f;
        }
        if (other.gameObject.CompareTag("TopSurfaceCollider"))
        {
            GetComponentInParent<ItemBehaviour>().isItemOnTopOfSurface = false;
            //GetComponentInParent<ItemBehaviour>().holdItemHigher -= 0.3f;
        }
        //if (collidedWithItem)
        {
            //collidedWithItem = false;
            //GetComponentInParent<ItemBehaviour>().holdItemHigher -= 0.4f;
        }
    }
}
