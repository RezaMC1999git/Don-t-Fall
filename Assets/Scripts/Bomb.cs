using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bomb : MonoBehaviour
{
    bool explotionStarted = false;
    [SerializeField] AudioSource expoltionSFX;
    [SerializeField] AudioSource bombCountdownSFX;
    [SerializeField] ParticleSystem explotionVFX;
    [SerializeField] TextMeshPro bombCountdown;
    [SerializeField] GameObject[] bombsGFX;
    [SerializeField] float bombColliderRange = 0.4f;

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Item") || other.gameObject.CompareTag("Fruit") ||
            other.gameObject.CompareTag("Food")) && !other.gameObject.GetComponent<ItemBehaviour>().CanControllItem)
        {
            if (!explotionStarted)
                StartCoroutine(ExplodeBomb());
        }
    }
    IEnumerator ExplodeBomb()
    {
        explotionStarted = true;
        bombCountdownSFX.Play();
        bombCountdown.gameObject.SetActive(true);
        bombCountdown.text = "5";
        yield return new WaitForSeconds(1f);
        bombCountdown.text = "4";
        yield return new WaitForSeconds(1f);
        bombCountdown.text = "3";
        yield return new WaitForSeconds(1f);
        bombCountdown.text = "2";
        yield return new WaitForSeconds(1f);
        bombCountdown.text = "1";
        yield return new WaitForSeconds(1f);
        bombCountdownSFX.Stop();
        bombCountdown.gameObject.SetActive(false);
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().isTrigger = false;
        GetComponent<SphereCollider>().radius = 1.5f;
        foreach(GameObject gfx in bombsGFX)
        {
            gfx.SetActive(false);
        }
        expoltionSFX.Play();
        explotionVFX.Play();
        yield return new WaitForSeconds(1.5f);
        explotionVFX.Stop();
        foreach (GameObject gfx in bombsGFX)
        {
            gfx.SetActive(true);
        }
        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<SphereCollider>().radius = bombColliderRange;
        explotionStarted = false;
    }
}
