using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Steers the ptz plane based on the gyro controller
/// </summary>
public class DisplayControlScript : MonoBehaviour {

    public GyroToControllsScript gyroController;
    public PlaneRotationScript ptzPlane;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        ptzPlane.MoveTo(new Vector2(gyroController.CamAngleX, -gyroController.CamAngleY));
        /*if (gyroController.gyroDataNew)
        {
            ptzPlane.MoveTo(new Vector2(gyroController.GyroAngleX, gyroController.GyroAngleY));
        }*/
	}
}
