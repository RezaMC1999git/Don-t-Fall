using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperOffTrigger : MonoBehaviour
{
    public Material normalDiskMaterial;
    public Material SlipperyDiskMaterial;
    public PhysicMaterial slipperyPhysicMaterial;
    public GameObject slipperyDisk;
    [SerializeField] LevelParameters levelParameters;
    private void Start()
    {
        InitialSlipperyDisk();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Surface"))
        {
            slipperyDisk.GetComponent<MeshRenderer>().material = normalDiskMaterial; 
            slipperyDisk.GetComponent<MeshCollider>().material = null;
            slipperyDisk.GetComponent<MeshCollider>().enabled = false;
            GetComponent<Animator>().SetTrigger("ToggleButton");
            StartCoroutine(SetActive());
        }
    }
    public void InitialSlipperyDisk()
    {
        slipperyDisk.GetComponent<MeshRenderer>().material = SlipperyDiskMaterial;
        slipperyDisk.GetComponent<MeshCollider>().material = slipperyPhysicMaterial;
        slipperyDisk.GetComponent<MeshCollider>().enabled = true;
    }
    IEnumerator SetActive()
    {
        FindObjectOfType<AudioSourcesController>().SFX[0].Play();
        gameObject.GetComponentInParent<MeshCollider>().isTrigger = true;
        gameObject.GetComponent<MeshCollider>().isTrigger = true;
        yield return new WaitForSeconds(1f);
        levelParameters.SlipperButtonActive = false;
        transform.parent.gameObject.SetActive(false);
    }
}
