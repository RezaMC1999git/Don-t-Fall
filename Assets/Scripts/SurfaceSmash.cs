using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceSmash : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject pauseButton;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")    //RewindTime
        {
            Application.LoadLevel(Application.loadedLevel);
            //transform.rotation = Quaternion.RotateTowards(transform.localRotation,Quaternion.Euler(0f, 0f, 0f),5f);
            //GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            //GetComponentInParent<Animator>().enabled = true;
            //GetComponentInParent<Animator>().SetTrigger("TouchedGround");
            //foreach (GameObject item in itemsParent)
            //    Destroy(item);
            //StartCoroutine(RewindTime());
        }

    }
    IEnumerator RewindTime()
    {
        gameObject.GetComponent<TimeBody>().startRewind = true;
        gameObject.GetComponent<TimeBody>().continueCallculating = false;
        pauseButton.SetActive(false);
        //Time.timeScale = 2f;
        yield return new WaitForSeconds(gameObject.GetComponent<TimeBody>().timeElapsed *2);
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        gameObject.GetComponent<TimeBody>().startRewind = false;
        //gameObject.GetComponent<TimeBody>().continueCallculating = true;
        gameObject.GetComponent<TimeBody>().CalculateTime();
    }
}
