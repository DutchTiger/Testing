using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipIndicator : MonoBehaviour {
    public Text ShipName;

	// Use this for initialization
	void Start () {
        ShipName = GetComponent<Text>();
        Destroy(gameObject,0.01f);
    }
	
    public void SetText(string text)
    {
        Debug.Log("SetText = " + text);
        ShipName.text = text;
    }
}
