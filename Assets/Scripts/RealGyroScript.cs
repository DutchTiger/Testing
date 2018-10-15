using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A real gyroscope
/// </summary>
public class RealGyroScript : GyroScript {


    private bool gyroEnabled; //Is the gyroscope enabled
    private Gyroscope gyro; //The gyroscope
    Vector3 lastPosition;
    bool hasNewData = true;


    private Ryan ryan; //Our private Ryan. 

    /// <summary>
    /// Turn on the gyro
    /// </summary>
    void Start()
    {
       
        Input.gyro.enabled = true;
        gyroEnabled = EnableGyro();
        ryan = new Ryan();
        SaveRyan(ryan);

    }

    /// <summary>
    /// 5/7 movie, perfect rating. 
    /// </summary>
    public void SaveRyan(Ryan ryan)
    {
        print("Saving Private Ryan...");
        ryan.Save();
    }

    /// <summary>
    /// Turn on the gyroscope and set the interval
    /// </summary>
    /// <returns>Whether it succeeded</returns>
    private bool EnableGyro()
    {
        if (SystemInfo.supportsGyroscope)
        {
            gyro = Input.gyro;
            gyro.enabled = true;
            gyro.updateInterval = 0.1f;// Time.fixedDeltaTime;
            return true;
        }
        return false;
    }

    private void FixedUpdate()
    {
        hasNewData = false;
        if (lastPosition != Input.gyro.rotationRateUnbiased)
        {
            lastPosition = Input.gyro.rotationRateUnbiased;
            hasNewData = true;
        }
    }

    /// <summary>
    /// Get the data of the gyroscope
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetData()
    {
        if (gyroEnabled)
        {
            return new Vector3(-lastPosition.y, lastPosition.x, 0);
        }
        return base.GetData();
    }

    public override bool HasNewData
    {
        get
        {
            return hasNewData;
        }
    }
}
