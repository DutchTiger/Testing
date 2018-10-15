using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A fake gyroscope.
/// </summary>
public class SimulatedGyroscopeScript : GyroScript {

    float xAxis, yAxis, zAxis; //The three axis
    float dXAxis, dYAxis, dZAxis; //Difference in the axis
    float t = 0; //Time

    // Use this for initialization
    void Start () {
        xAxis = 0;
        yAxis = 0;
        zAxis = 0;

        dXAxis = 0;
        dYAxis = 0;
        dZAxis = 0;
		
	}

	/// <summary>
    /// Generate new gyroscope output
    /// </summary>
	void FixedUpdate () {

        t += 0.06f;
        dXAxis += Random.Range(-0.01f, 0.01f);
        dYAxis += Random.Range(-0.01f, 0.01f);
        dZAxis += Random.Range(-0.01f, 0.01f);
        if (dXAxis > 0.03f * (1 + Mathf.Cos(t)))
        {
            dXAxis = 0.03f * (1 + Mathf.Cos(t));
        }
        if (dXAxis < -0.03f * (1 + Mathf.Cos(t)))
        {
            dXAxis = -0.03f * (1 + Mathf.Cos(t));
        }
        if (dYAxis > 0.03f * (1 + Mathf.Cos(t)))
        {
            dYAxis = 0.03f * (1 + Mathf.Cos(t));
        }
        if (dYAxis < -0.03f * (1 + Mathf.Cos(t)))
        {
            dYAxis = -0.03f * (1 + Mathf.Cos(t));
        }
        if (dZAxis > 0.03f * (1 + Mathf.Cos(t)))
        {
            dZAxis = 0.03f * (1 + Mathf.Cos(t));
        }
        if (dZAxis < -0.03f * (1 + Mathf.Cos(t)))
        {
            dZAxis = -0.03f * (1 + Mathf.Cos(t));
        }


        xAxis += dXAxis;
        yAxis += dYAxis;
        zAxis += dZAxis;

        if (xAxis > 6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            xAxis = 6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
        if (xAxis < -6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            xAxis = -6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
        if (yAxis > 6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            yAxis = 6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
        if (yAxis < -6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            yAxis = -6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
        if (zAxis > 6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            zAxis = 6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
        if (zAxis < -6 * (1 + Mathf.Cos(t + Mathf.PI)))
        {
            zAxis = -6 * (1 + Mathf.Cos(t + Mathf.PI));
        }
    }

    /// <summary>
    /// Get the data from the gyroscope.
    /// </summary>
    /// <returns></returns>
    public override Vector3 GetData()
    {
        return new Vector3(xAxis, yAxis, zAxis);
    }
}
