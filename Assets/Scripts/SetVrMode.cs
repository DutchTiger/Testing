using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// This script sets the mode to Non VR-Mode or VR-Mode
/// </summary>
public class SetVrMode : MonoBehaviour {
    
    public static string[] supportedDevices;

    private void Awake()
    {
        //Retrieves list of supported VR devices which can be found in Unity at: "Edit/Project Setting/Player" under the XRSettings section
        supportedDevices = XRSettings.supportedDevices;
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the scene when Android close icon or back button tapped.
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Sets mode to Non-VR Mode
    /// </summary>
    /// <returns></returns>
    public static IEnumerator SetToNonVrMode()
    {
        XRSettings.LoadDeviceByName("none");
        yield return null; // wait one frame
    }

    /// <summary> 
    /// Sets mode to VR Mode
    /// </summary>
    /// <returns></returns>
    public static IEnumerator SetToVrMode()
    {
        XRSettings.LoadDeviceByName(supportedDevices[1]);
        yield return null; // wait one frame
        XRSettings.enabled = true;
    }

}
