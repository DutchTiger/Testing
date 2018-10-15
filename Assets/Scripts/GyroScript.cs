using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a abstract class for a gyroscope.
/// </summary>
public class GyroScript : MonoBehaviour {

    /// <summary>
    /// Returns the data of the gyroscope.
    /// </summary>
    /// <returns>Change in degrees since last update</returns>
    public virtual Vector3 GetData()
    {
        return new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Whether or not the gyroscope has been updated since it was last asked for data.
    /// </summary>
    public virtual bool HasNewData
    {
        get
        {
            return true;
        }
    }
}
