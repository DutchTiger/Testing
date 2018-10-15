using UnityEngine;
using System;

/// <summary>
/// A Unity3D Script to dipsplay Mjpeg streams. Apply this script to the mesh that you want to use to view the Mjpeg stream. 
/// This script also retrieves the urls created in the script: "CamIP_Config"
/// </summary>
public class MjpegTexture : MonoBehaviour
{
	/// <param name="streamAddress">
	/// Set this to be the network address of the mjpg stream. 
	/// Example: "http://extcam-16.se.axis.com/mjpg/video.mjpg"
	/// </param>
   
    byte[] pixelMap;
    const int numOfCols = 16;
    const int numOfRows = numOfCols / 2;
	const int numOfPixels = numOfCols * numOfRows;

    // Flag showing when to update the frame
    bool updateFrame = false;

    // PTZ
    private string ptzStreamAddress = "http://192.168.2.3:8881/video.mjpg";
    Texture2D texPTZ;
    MjpegProcessor mjpegPTZ;

    // Theta
    private string thetaStreamAddress = "http://192.168.2.3:8881/video.mjpg";
    Texture2D texShpere;
    MjpegProcessor mjpegTheta;
    GameObject sphere;

    int frameCount = 0;

   public void Start() {

        // PTZ. 
        ptzStreamAddress = CamIP_Config.streamUrl + "video.mjpg";
        mjpegPTZ = new MjpegProcessor();
		mjpegPTZ.FrameReady += mjpeg_FrameReady;
		mjpegPTZ.Error += mjpeg_Error;
        Uri mjpeg_address = new Uri(ptzStreamAddress);
		mjpegPTZ.ParseStream(mjpeg_address);
        texPTZ = new Texture2D(1920, 1080); // PTZ texture

        // Theta
        //thetaStreamAddress = CamIP_Config.streamUrl_360 + "video.mjpg";
        mjpegTheta = new MjpegProcessor();
        mjpegTheta.FrameReady += mjpeg_FrameReady;
        mjpegTheta.Error += mjpeg_Error;
        Uri mjpegTheta_address = new Uri(thetaStreamAddress);
        mjpegTheta.ParseStream(mjpegTheta_address);
        texShpere = new Texture2D(1920, 1080); // 360 Sphere texture
  
        // Sphere object
        sphere = GameObject.FindWithTag("Sphere");
    }
    private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e) {
        updateFrame = true; // Mjpeg frame is ready. Enable update
    }
    void mjpeg_Error(object sender, ErrorEventArgs e) {
        Debug.Log("Error received while reading the MJPEG.");
    }
    
    // Update is called once per frame
    void Update() {
        if (updateFrame) {
            // Load and set PTZ texture
		    texPTZ.LoadImage(mjpegPTZ.CurrentFrame);
            texPTZ.Apply();
            // Assign texture to screen pane
            GetComponent<Renderer>().material.mainTexture = texPTZ;

            // Load and set Sphere texture
            texShpere.LoadImage(mjpegTheta.CurrentFrame);
            texShpere.Apply();
            // Assign texture to sphere

            // By checking the width and heigh of the texture before it is assigns prevents the weird questionmark rendering for showing up
            if (texShpere.width > 100 && texShpere.height > 100)
            {
                sphere.GetComponent<Renderer>().material.mainTexture = texShpere;
            } else
            {
                sphere.GetComponent<Renderer>().material.mainTexture = Resources.Load("Above the sea") as Texture;
            }
            
            updateFrame = false;
        }
    }

    void OnDestroy() {
       // Stop the mjpeg streams
		mjpegPTZ.StopStream();
        mjpegTheta.StopStream();
    }
}
