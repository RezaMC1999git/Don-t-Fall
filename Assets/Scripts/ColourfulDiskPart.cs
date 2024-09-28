using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourfulDiskPart : MonoBehaviour
{
    LevelParameters levelParameters;
    public int howManyColorfulDisksCollidedWith = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ItemBehaviour>() != null)
        {
            if (other.GetComponent<ItemBehaviour>().triggerd == false)
            {
                if (other.gameObject.tag == "Red" || other.gameObject.tag == "Green" ||
                    other.gameObject.tag == "Blue" || other.gameObject.tag == "Yellow")
                {
                    levelParameters = other.gameObject.GetComponent<ItemBehaviour>().levelParameters;
                    if (other.gameObject.tag != this.gameObject.tag)
                    {
                        levelParameters.IncreaseNumberOfItem(other.gameObject.name);
                        levelParameters.numberOfAllItems++;
                        other.GetComponent<ItemBehaviour>().triggerd = true;
                        StartCoroutine(DestroyItem(other.gameObject));
                    }
                }
            }
        }
    }
    IEnumerator DestroyItem(GameObject item)
    {
        ParticleSystem dParticle = Instantiate(item.GetComponent<ItemBehaviour>().deathParticle,
            item.transform.position, Quaternion.EulerAngles(-90f,0f,0f));
        dParticle.Play();
        Destroy(item);
        yield return new WaitForSeconds(2.5f);
        //Destroy(dParticle);
        //dParticle.Stop();
    }
}
