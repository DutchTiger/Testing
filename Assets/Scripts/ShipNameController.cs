using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNameController : MonoBehaviour {
    private static ShipName shipName;

    //create an indicator that tells you a ship's in that direction with it's name.
    public static void Initialize()
    {
        Debug.Log("ShipNameController Initialized");
        if (!shipName)
        {
            shipName = Resources.Load<ShipName>("Prefab/ShipIndicator3D");
        }

    }

    public static void CreateShipName(string text, Transform camLocation, Transform shipLocation)
    {
        Debug.Log("CreateSNI text = " + text + " Location = " + camLocation.position);
        ShipName instance = Instantiate(shipName);
        instance.transform.position = Vector3.Lerp(camLocation.position, shipLocation.position, 0.1f);
        instance.transform.LookAt(Camera.main.transform, new Vector3(0,0,0));
        //instance.transform.localEulerAngles = new Vector3(180, 0, 0);
        instance.SetText(text);
    }
}
