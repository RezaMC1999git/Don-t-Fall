using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] Rigidbody m_Rigidbody;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
