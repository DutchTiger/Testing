using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipIndicatorController : MonoBehaviour {
    private static ShipIndicator shipName;
    private static GameObject canvas;

    public static void Initialize()
    {
        Debug.Log("ShipIndicatorController Initialized");
        canvas = GameObject.Find("Canvas");
        if (!shipName)
        {
            shipName = Resources.Load<ShipIndicator>("Prefab/ShipIndicator");
        }

    }
	public static void CreateShipNameIndicator(string text, Transform location)
    {
        Debug.Log("CreateSNI text = " + text + " Location = " + location.position);
        ShipIndicator instance = Instantiate(shipName);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(location.position);
        Debug.Log("ScreenPosition = " + screenPosition);
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screenPosition;
        instance.SetText(text);
    }
}
