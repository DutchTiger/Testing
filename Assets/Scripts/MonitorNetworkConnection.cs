using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Monitors the network connection( currently only for one camera )
/// </summary>
public class MonitorNetworkConnection : MonoBehaviour {

    private string url_link;
    private float pingInterval = 5f;//5 seconds
    public Image lostConnectionMsg;
    private int framesPassed = 0;
    private int frameThreshold = 200; 

	// Use this for initialization
	void Start () {
        lostConnectionMsg.enabled = false;
        url_link = CamIP_Config.streamUrl;
    }

    /// <summary>
    /// Update is called once per frame.
    /// Checks the network connection after 200 frames. Onced Checked it waits another 200 frames before checking again.  
    /// </summary>
    void Update () {

        if (framesPassed == frameThreshold)
        {
            StartCoroutine(CheckNetworkConnection());             
        }
        framesPassed++;
	}

    /// <summary>
    /// Checks connection currently only for one camera
    /// </summary>
    /// <returns></returns>
    public IEnumerator CheckNetworkConnection()
    {        
        WWW www = new WWW(url_link);        
        
            yield return www;

            if (www.error != null)
            {
                lostConnectionMsg.enabled = true;
                yield return new WaitForSeconds(pingInterval);
                SceneManager.LoadScene(0);       
            }
            else
            {
                print("Connection good ");
            }
        framesPassed = 0;
    }

}
