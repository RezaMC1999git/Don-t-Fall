using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCountdown : MonoBehaviour
{
    [SerializeField] Transform target;
    void Update()
    {
        transform.localRotation = Quaternion.Euler(target.eulerAngles.x, target.eulerAngles.y, 0f);   
    }
}
