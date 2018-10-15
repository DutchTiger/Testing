using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using UnityEngine.XR;
using System;

/// <summary>
/// This script is used for the camera configuration in the UI(the first scene). 
/// This script takes the UI input of both the PTZ as well as the 360 camera's panel and 
/// has a URL created per panel and checks if a connection can be made with the camera's.
/// 
/// NOTE:
/// After the input is validated and a URL is created, the URL is assigned to a static variable,
/// which could then dynamically be used by the rest of the system/application. 
/// </summary>
public class CamIP_Config : MonoBehaviour {
    
    public class Cam
    {
        public string ipAddress;
        public int portNum;

        public Cam(string ip, string port)
        {
            ipAddress = ip;
            int.TryParse(port, out portNum);// converts string to int 
        }        
    }

    public InputField inputIp_1;
    public InputField inputPort_1;
    public InputField inputIp_2;
    public InputField inputPort_2;
    public Cam cam1;
    public Cam cam2;
    public Camera mainCamera;
    public Button connectButton1;
    public Button connectButton2;
    public Button proceedButton;
    public Text display;
    public Text connectStatus_Display;
    public Image panel_1;
    public Image panel_2;
    public static string streamUrl; //PTZ camera url.
    public static string streamUrl_360; //360 camera url.

    private int[] camSelection = { 1, 2 };
    private Color default_color = new Color32(255, 255, 255, 100); //Grey
    private Color success_color = new Color32(86, 255, 86, 120); //Green
    private Color failure_color = new Color32(255, 60, 60, 147); //Red
    private bool isConnected;
    private int framesPassed = 0;
    private bool isConnecting;

    int maxPortNum = 65535;
    int invalidPortNum = 0;
    
    /// <summary>
    /// Use this for initialization
    /// </summary>
    /// <param name=""></param>
    /// <param name="Start"></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    void Start()
    {
        StartCoroutine(SetVrMode.SetToNonVrMode());//Allows app to start in non-VR mode
        connectStatus_Display.text = "The application is not connected to a camera. Please fill in the ip address and portnumber of the camera you wish to connect with:";
        isConnected = false;    
       
        connectButton1.onClick.AddListener(() => SetupCamera(camSelection[0]));

        //Disabled the button because dynamically created url for the 360 cam lead to some issues.Hardcoded url was used instead
        //connectButton2.onClick.AddListener(() => SetupCamera(camSelection[1])); 
        proceedButton.onClick.AddListener(() => ProceedToStream());
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        //Enables the proceed button after a successful connection with camera 
        if (isConnected)
        {
            proceedButton.interactable = true;
        }
        else
        {
            proceedButton.interactable = false;
        }

        if (framesPassed < 20 )
        {            
            ResetAspectRatio();           
        }

         if(isConnecting)
        {
            LoadingCircle.Show();
        }    
    }

    /// <summary>
    /// This function is used to reset the aspect ratio of the camera after exiting VR mode.
    /// Without this, the screen stretches horizontally. When switching to VR mode the Field of View and aspect ratio
    /// are automatically altered. However, only the FoV is reset after switching back from VR mode to Non-VR Mode.
    /// </summary>
    public void ResetAspectRatio()
    {
        Rect rect = mainCamera.rect;
        rect.width = Screen.width;
        rect.height = Screen.height;
        mainCamera.rect = rect;
        framesPassed++;
    }    

    /// <summary>
    /// Sets up the select camera.
    /// Retrieves and verfies input, creates url and checks if the url can be reached. 
    /// </summary>
    /// <param name="selected"></param>
    public void SetupCamera(int selected)
    {
        if (selected == camSelection[0])
        {
            cam1 = new Cam(inputIp_1.text, inputPort_1.text);            

            if (VerifyInput(cam1))
            {                
                streamUrl = GeneralMethods.CreateURL(cam1.ipAddress, cam1.portNum);
                StartCoroutine(CheckNetworkConnection(streamUrl,panel_1));
            }
            else
            {
                panel_1.color = default_color;
                isConnected = false;
            }
        }
        else
        {
            cam2 = new Cam(inputIp_2.text, inputPort_2.text);

            if (VerifyInput(cam2) == true)
            {
                streamUrl_360 = GeneralMethods.CreateURL(cam2.ipAddress, cam2.portNum);
                StartCoroutine(CheckNetworkConnection(streamUrl_360,panel_2));
            }
            else
            {
                panel_2.color = default_color;
                isConnected = false;
            }
        }
    }    

    /// <summary>
    /// This method uses Regex (Regular Exclusion) tto check if IP input is a valid IP address.
    /// 
    /// </summary>
    /// <param name="cam"></param>
    /// <returns> Whether or not the input is verified </returns>
    public bool VerifyInput(Cam cam)
    {
        //Regex which allows the following pattern -> [0-255].[0-255].[0-255].[0-255] (Found on the internet)             
        string ipPattern = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";


        //Checks if ip address is filled in or if the port number is valid 
        if (cam.portNum > invalidPortNum && cam.portNum <= maxPortNum && cam.ipAddress != "")
        {
            if (Regex.IsMatch(cam.ipAddress, ipPattern))
            {
                display.text = "address is: " + cam.ipAddress + " and port is: " + cam.portNum;
                return true;
            }
            else
            {
                display.text = "Invalid IP Address. The IP Address Must Have The Following Pattern: [0-255].[0-255].[0-255].[0-255]";
                return false;
            }
        }

        else if (cam.portNum == invalidPortNum || cam.portNum > maxPortNum && cam.ipAddress != "")
        {
            if (Regex.IsMatch(cam.ipAddress, ipPattern))
            {
                display.text = "Port Number is either empty or invalid!";
            }
            else display.text = "Invalid IP Address and invalid or empty Port Number.";

            return false;
        }

        else
        {
            display.text = "Both fields must be filled in!";
            return false;
        }
    }

    /// <summary>
    /// Checks if a connection can be made with the url created.
    /// </summary>
    /// <param name="url_link"></param>
    /// <param name="panel"></param>
    /// <returns>The result of the connection attempt </returns>
    public IEnumerator CheckNetworkConnection(string url_link, Image panel)
    {
        WWW www = new WWW(url_link);

        connectStatus_Display.text = "Trying to connect to camera...";       
        isConnecting = true;
        yield return www; //pauses further execution until connection is made or has failed 

        if (www.error != null)
        {
            LoadingCircle.Hide();
            panel.color = failure_color;
            connectStatus_Display.text = "The application was unable to connect with the camera. Please fill in the ip address and portnumber of the camera you wish to connect with or check the connection:";
            isConnecting = false; 
            isConnected = false;
        }
        else
        {
            LoadingCircle.Hide();
            panel.color = success_color;
            connectStatus_Display.text = "Connection with camera was successful!";
            isConnecting = false;
            isConnected = true;
        }
    }

    /// <summary>
    /// Method excecuted by proceed button, which changes the scene. 
    /// </summary>
    public void ProceedToStream()    {

        SceneManager.LoadScene(1);
    }



        
}
