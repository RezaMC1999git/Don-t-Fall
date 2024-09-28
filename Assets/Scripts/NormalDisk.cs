using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDisk : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            if (other.gameObject.GetComponent<MeshCollider>() != null)
                other.gameObject.GetComponent<MeshCollider>().material = null;
            if (other.gameObject.GetComponent<BoxCollider>() != null)
                other.gameObject.GetComponent<BoxCollider>().material = null;
        }
    }
}
