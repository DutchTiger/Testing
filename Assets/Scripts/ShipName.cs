using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipName : MonoBehaviour {
    public TextMesh ShipNameMesh;

    // small script to change the text so you can see the name of the ship.
    void Start()
    {
        ShipNameMesh = GetComponent<TextMesh>();
    }

    public void SetText(string text)
    {
        Debug.Log("SetText = " + text);
        ShipNameMesh.text = text;
    }
}
