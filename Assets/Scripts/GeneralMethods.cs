using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Static class with a bunch of methods that don't fit anywhere else. 
/// </summary>
public static class GeneralMethods {
    
    public static Vector2 WrongReturnAngle = new Vector2(69, 420); // Used as a nonexistant angle in case an angle cannot be set.
    
    /// <summary>
    /// Overflow an angle to be between 0 and 360
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static float OverFlowAngle(float angle, bool radians = false)
    {
        float max = 360;
        if (radians)
        {
            max = Mathf.PI * 2;
        }
        angle %= max;
        if (angle < 0)
        {
            angle += max;
        }
        return angle;
    }


    /// <summary>
    /// Converts an angle from degrees to radians or back.
    /// </summary>
    /// <param name="angle">Angle to convert</param>
    /// <param name="reverse">if true, go from radians to degrees</param>
    /// <returns></returns>
    public static float AngleToRadian(float angle, bool reverse = false)
    {
        if (reverse)
        {
            return GeneralMethods.OverFlowAngle(angle, true) * 180f / Mathf.PI;
        }
        return GeneralMethods.OverFlowAngle(angle) / 180f * Mathf.PI;
    }

    /// <summary>
    /// Finds the true difference between 2 angles.
    /// </summary>
    /// <param name="angle1"></param>
    /// <param name="angle2"></param>
    /// <returns></returns>
    public static float AngularDifference(float angle1, float angle2)
    {
        angle1 = OverFlowAngle(angle1);
        angle2 = OverFlowAngle(angle2);

        float difference = OverFlowAngle(angle2 - angle1);
        float difference2 = OverFlowAngle(angle1 - angle2);

        if (difference > difference2)
        {
            return difference2;
        }
        return difference;
    }

    /// <summary>
    /// Create a URL from ipp adress, portnum and a path
    /// </summary>
    /// <param name="ipaddress"></param>
    /// <param name="portnum"></param>
    /// <param name="path"></param>
    /// <returns>The URL</returns>
    public static string CreateURL(string ipaddress, int portnum, string path = "")
    {
        string url = "http://";
        url = url + ipaddress + ":" + portnum + "/" + path;
        return url;
    }

}
