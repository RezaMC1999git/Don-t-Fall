using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour
{
    public bool isRewinding = false;
    public float revindTime = 5f;
    public bool startRewind = false;
    public bool continueCallculating = false;
    public float timeElapsed = 0f;
    public List<PointInTime> pointsInTime;
    Rigidbody rb;
    void Start()
    {
        pointsInTime = new List<PointInTime>();
        pointsInTime.Clear();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isRewinding)
            Rewind();
        else
            Record();
        if (startRewind)
            StartRewind();
        if (!startRewind)
            StopRewind();
    }
    private void FixedUpdate()
    {

    }
    public void StartRewind()
    {
        isRewinding = true;
        rb.isKinematic = true;
    }
    public void StopRewind()
    {
        isRewinding = false;
        rb.isKinematic = false;
    }
    void Rewind()
    {
        if(pointsInTime.Count > 0)
        {
            Time.timeScale = 2f;
            PointInTime pointInTime = pointsInTime[0]; 
            transform.position = pointInTime.position;
            transform.rotation = pointInTime.rotation;
            pointsInTime.RemoveAt(0);
        }
        StopRewind();
    }
    void Record()
    {
        if(pointsInTime.Count > Mathf.Round (timeElapsed / Time.fixedDeltaTime))
        {
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        if(pointsInTime.Count == 0)
            pointsInTime.Insert(0, new PointInTime(new Vector3(0f,6f,0f), Quaternion.Euler(0f,0f,0f)));
        pointsInTime.Insert(0, new PointInTime(transform.localPosition,transform.localRotation));
    }
    public void CalculateTime()
    {
        timeElapsed = 0f;
        continueCallculating = true;
        StartCoroutine(TimeCounter());
    }
    IEnumerator TimeCounter()
    {
        if (!continueCallculating)
            yield break;
        yield return new WaitForSeconds(1f);
        if (continueCallculating)
        {
            timeElapsed++;
            StartCoroutine(TimeCounter());
        }
        else
            yield break;
    }
}
