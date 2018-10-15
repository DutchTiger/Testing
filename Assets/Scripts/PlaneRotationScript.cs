using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Rotates the plane around a certain point.
/// </summary>
public class PlaneRotationScript : MonoBehaviour {
    Vector2 currentAngles = new Vector2();

    //TODO mogelijk kan ik linear interpoleren tussen vorige value en net net verzonden gyroscoop data, gezien dat is waar ie heen zal bewegen. 

    Vector3 middlePoint = new Vector3();
    float distance = 10.6f;
    float maxRotationRate = 140f;

    IEnumerator movingClock;

    private void Start()
    {
        currentAngles.x = transform.rotation.x;
        currentAngles.y = transform.rotation.y;
    }

    public void SetAngles(Vector2 angles)
    {
        transform.rotation = Quaternion.Euler(angles.y, angles.x, 0);
    }

    public void MoveTo(Vector2 newAngles)
    {
        /*float angularDifference = Mathf.Sqrt(Mathf.Pow(GeneralMethods.AngularDifference(currentAngles.x, newAngles.x), 2) + Mathf.Pow(GeneralMethods.AngularDifference(currentAngles.y, newAngles.y), 2));
        float time = angularDifference / maxRotationRate;

        if (movingClock != null)
        {
            StopCoroutine(movingClock);
            movingClock = null;
        }

        movingClock = MoveToItterative(currentAngles, newAngles, time, 0);
        StartCoroutine(movingClock);*/
        SetAngles(newAngles);
    }

    /// <summary>
    /// Linear interpolation from start to end over time
    /// </summary>
    /// <param name="start">Position where to start</param>
    /// <param name="end">Position where to end</param>
    /// <param name="time">Total time it should take to get from start to end</param>
    /// <param name="totalTime">Time thus far</param>
    /// <returns></returns>
    private IEnumerator MoveToItterative(Vector2 start, Vector2 end, float time, float totalTime)
    {
        currentAngles = (start / time * (time - totalTime)) + end / time * totalTime;
        SetAngles(currentAngles);
        yield return new WaitForEndOfFrame();
        totalTime += Time.deltaTime;

        if (totalTime < time)
        {
            movingClock = MoveToItterative(start, end, time, totalTime);
            StartCoroutine(movingClock);
        }
        else
        {
            SetAngles(end);
            movingClock = null;
        }
    }
    


    public float /*Koeken*/Pan
    {
        get
        {
            return currentAngles.x;
        }
    }

    public float Tilt
    {
        get
        {
            return currentAngles.y;
        }
    }
}
