using UnityEngine;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This script controls the PTZ camera at Decipher.
/// </summary>
public class DecipherPTZCam : CameraControllScript
{

    //This url needs to be replaced with the url created in the CamIP_config script, which has a "/" at the end of it 
    private string url = "http://192.168.2.15:8081"; //URL to the camera.
    //private string url = "http://192.168.21.104:80"; //URL to the camera.

    public int messagesSent = 0; //Keeping track of how many messages have been sent.
    float minPanPosition = 1;
    float maxPanPosition = 35199;
    float minTiltPosition = 0;
    float maxTiltPosition = 8800;
    float degreesToPanMultiplier;
    float degreesToTiltMultiplier;

    public int maxAttemptsCommand = 5;


    //================!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //All the zooming functionalty in this script should be moved to a general UI functions script or isolated into its own and then be referenced from there 

    public Slider zoomSlider; //TODO dit hoort hier niet.
    public Text zoomText; //dit ook niet.

    //  Zoom levels... 16384(Max Zoom)/4 = 4094 per zoom level 
    string zoomLevel_0 = "/cgi-bin/camctrl/camposition.cgi?setzoom=0";//Zoom starting point.
    string zoomLevel_1 = "/cgi-bin/camctrl/camposition.cgi?setzoom=4094";
    string zoomLevel_2 = "/cgi-bin/camctrl/camposition.cgi?setzoom=8188";
    string zoomLevel_3 = "/cgi-bin/camctrl/camposition.cgi?setzoom=12282";
    string zoomLevel_4 = "/cgi-bin/camctrl/camposition.cgi?setzoom=16384";//Max zoom

    static int currentZoomLevel;
    static int zoomStatus = 0;

    //status of zoom buttons 
    int maxStatus = 4;
    int minStatus = 0;

    int[] zoomVal = { 0, 4094, 8188, 12282, 16384 };

    //private SetVrMode vrSetter; 

    /// <summary>
    /// Set the known rotation speeds. And the zoom level is set to minimal zoom when the application starts up.
    /// </summary>
    protected override void Start()
    {
        //vrSetter = new SetVrMode();
        StartCoroutine(SetVrMode.SetToVrMode());

        knownXRotationSpeeds = new double[] { 0.773, 1.93, 1.93, 4.20, 7.08, 10.58, 15.29, 24.905, 45.23, 84, 152 };
        knownYRotationSpeeds = new double[] { 0.15625, 0.3125, 0.625, 1.25, 2.5, 5, 10, 20, 40, 80, 160 };

        degreesToPanMultiplier = 360.0f / (maxPanPosition - minPanPosition);
        degreesToTiltMultiplier = 90f / (maxTiltPosition - minTiltPosition);

        SetSliderValue(zoomVal[0]);
        WWW www = new WWW(url + zoomLevel_0);
        SetZoomText(zoomStatus);
    }

    /// <summary>
    /// Allows application to constantly check the input of the zoom buttons 
    /// </summary>
    public override void Update()
    {
        ZoomInput();
    }

    /// <summary>
    /// If any changes are made to the direction or speed, set the direction and speed of the camera.
    /// </summary>
    public override void FixedUpdate()
    {
        if (anyChangesMade)
        {
            SetDirections();
        }
        anyChangesMade = false;

    }

    /// <summary>
    ///  Have the camera set its zoomlevel based on which button is pressed
    /// </summary>
    public void ZoomInput()
    {
        //Zoom in if button 1 is pressed.
        if (Input.GetButtonDown("Fire1"))
        {
            zoomStatus++;
            if (zoomStatus > maxStatus)
            {
                zoomStatus = maxStatus;
            }

            ZoomCommands(zoomStatus);
            StartCoroutine(GetCurrentZoomCommand());
        }

        //Zoom out if button 2 is pressed. 
        if (Input.GetButtonDown("Fire2"))
        {
            zoomStatus--;
            if (zoomStatus < minStatus)
            {
                zoomStatus = minStatus;
            }

            ZoomCommands(zoomStatus);
            StartCoroutine(GetCurrentZoomCommand());
        }
    }

    /// <summary>
    /// Have the camera zoom in and out based on zoom level.
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public WWW ZoomCommands(int status)
    {
        int buttonStatus = status;
        string command = "";

        switch (buttonStatus)
        {
            case 0:
                command = (url + zoomLevel_0);
                break;
            case 1:
                command = (url + zoomLevel_1);
                break;
            case 2:
                command = (url + zoomLevel_2);
                break;
            case 3:
                command = (url + zoomLevel_3);
                break;
            case 4:
                command = (url + zoomLevel_4);
                break;
        }

        SetSliderValue(zoomVal[buttonStatus]);
        SetZoomText(buttonStatus);
        WWW www = new WWW(command);
        return www;
    }

    /// <summary>
    /// Set the value of the zoom slider
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetSliderValue(int value)
    {
        zoomSlider.value = value;
        return true;
    }

    /// <summary>
    /// Retrieves the current zoomlevel of the camera. Because the message can only be retrieved as a string, 
    /// the GetNumFromString() method is called. This function can be called by starting a coroutine --->  StartCoroutine(GetCurrentZoomCommand());  
    /// </summary>
    /// <returns></returns>
    public override IEnumerator GetCurrentZoomCommand()
    {
        CoroutineWithData cd = new CoroutineWithData(this, SendCommand("cgi-bin/camctrl/camposition.cgi?getzoom=1"));  //Getting current zoomlevel message
        yield return cd.coroutine;

        WWW www = (WWW)cd.result;
        //yield return new WaitUntil(() => www.isDone);

        currentZoomLevel = GetNumFromString(www.text);//extract and convert the received current zoomlevel to an int and assign it to the variable "currentZoomLevel".        
    }

    /// <summary>
    /// Splits the retrieved string into two separate strings and stores them in an array. The format of the message = "command=value". 
    /// Everything before the "=" is stored as the first string of the array(variable). While everything after is stored as the second string of the array(value).
    /// After doing this the value string is converted to an int.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public int GetNumFromString(string text)
    {
        int num = 0;
        string command = "";
        string value = "";

        string[] splitArray = text.Split(char.Parse("="));
        command = splitArray[0];
        value = splitArray[1];
        
        num = int.Parse(value);
        return num;
    }

    /// <summary>
    /// set the text of the zoom bar.
    /// </summary>
    /// <param name="level"></param>
    public void SetZoomText(int level)
    {
        switch (level)
        {
            case 0:
                zoomText.text = "1x";
                break;
            case 1:
                zoomText.text = "2x";
                break;
            case 2:
                zoomText.text = "3x";
                break;
            case 3:
                zoomText.text = "4x";
                break;
            case 4:
                zoomText.text = "5x";
                break;
        }
    }

    /// <summary>
    /// Send the comandos to the camera, according to what direction the camera should have.
    /// </summary>
    protected override void SetDirections()
    { //Lovely spagetti is unavoidable. just add some sauce, just sauce, raw sauce. boom, yo.
        if (up && !down)
        {
            if (left && !right)
            {
                StartCoroutine(TurnLeftUpCommand());
            }
            else if (right && !left)
            {
                StartCoroutine(TurnRightUpCommand());
            }
            else
            {
                StartCoroutine(TurnUpCommand());
            }
        }
        else if (down && !up)
        {
            if (left && !right)
            {
                StartCoroutine(TurnLeftDownCommand());
            }
            else if (right && !left)
            {
                StartCoroutine(TurnRightDownCommand());
            }
            else
            {
                StartCoroutine(TurnDownCommand());
            }
        }
        else if (left && !right && !up && !down)
        {
            StartCoroutine(TurnLeftCommand());
        }
        else if (right && !left && !up && !down)
        {
            StartCoroutine(TurnRightCommand());
        }
        else
        {
            StartCoroutine(TurnStopCommand());
        }
    }

    /// <summary>
    /// Set Camera panning speed (horizontal)
    /// </summary>
    /// <param name="speedMode">How fast it should be -5 to 5</param>
    public override void SetXSpeed(int speedMode)
    {
        if (speedMode >= -5 && speedMode <= 5) //only allow speeds between -5 and 5
        {
            if (xMultiplier != speedMode)
            {
                StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?speedpan=" + speedMode));
                xMultiplier = speedMode;
                anyChangesMade = true;
                ++messagesSent;
            }
        }
    }

    /// <summary>
    /// Set camera tilting speed (vertical)
    /// </summary>
    /// <param name="speedMode">How fast it should be -5 to 5</param>
    public override void SetYSpeed(int speedMode)
    {
        if (speedMode >= -5 && speedMode <= 5) //only allow speeds between -5 and 5
        {
            if (yMultiplier != speedMode)
            {
                StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?speedtilt=" + speedMode));
                yMultiplier = speedMode;
                anyChangesMade = true;
                ++messagesSent;
            }
        }
    }

    /// <summary>
    /// Have the camera zoom in.
    /// (NOT BEING USED)
    /// </summary>
    public IEnumerator ZoomInCommand()
    {
        CanBeControlled = false;
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?zoom=tele"));
    }

    /// <summary>
    /// Have the camera zoom out.
    /// (NOT BEING USED)
    /// </summary>
    public IEnumerator ZoomOutCommand()
    {
        CanBeControlled = false;
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?zoom=wide"));
    }


    /// <summary>
    /// Have the camera turn up.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TurnUpCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=0&vy=10"));
    }

    /// <summary>
    /// Have the camera turn down
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnDownCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=0&vy=-10"));
    }

    /// <summary>
    /// Have the camera turn left
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnLeftCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=-10&vy=0"));
    }

    /// <summary>
    /// Have the camera turn right
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnRightCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=10&vy=0"));
    }

    /// <summary>
    /// Have the camera stop moving
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnStopCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=0&vy=0"));
    }

    /// <summary>
    /// Have the camera turn right and up
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnRightUpCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=10&vy=10"));
    }

    /// <summary>
    /// Have the camera turn right and down
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnRightDownCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=10&vy=-10"));
    }

    /// <summary>
    /// Have the camera turn left and up
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnLeftUpCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=-10&vy=10"));
    }

    /// <summary>
    /// Have the camera turn left and down.
    /// </summary>
    /// <returns></returns>
	public IEnumerator TurnLeftDownCommand()
    {
        yield return StartCoroutine(SendCommand("/cgi-bin/camctrl/camctrl.cgi?vx=-10&vy=-10"));
    }

    /// <summary>
    /// Move to a location.
    /// </summary>
    /// <param name="x">X angle to move to, in degrees</param>
    /// <param name="y">Y angle to move to, in degrees</param>
    /// <returns></returns>
    public override IEnumerator MoveToLocation(float x, float y)
    {
        float pan = x / degreesToPanMultiplier + minPanPosition;
        float tilt = y / degreesToTiltMultiplier + minTiltPosition;
        pan = Mathf.Clamp(Mathf.Round(pan), minPanPosition, maxPanPosition);
        tilt = Mathf.Clamp(Mathf.Round(tilt), minTiltPosition, maxTiltPosition);
        string command = "/cgi-bin/camctrl/camposition.cgi?setpan=" + pan + "&settilt=" + tilt;

        CoroutineWithData cd = new CoroutineWithData(this, SendCommand(command));  
        yield return cd.coroutine;

        yield return ((WWW)cd.result).text;
        controller.PositionalCameraDoneMoving();
    }

    /// <summary>
    /// Send the command to get the current position of the camera.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator GetCurrentPositionCommand()
    {
        WWW www = new WWW(url + "/cgi-bin/camctrl/camposition.cgi?getpan=1&gettilt=1");
        ++messagesSent;
        float time = 0;
        while (!(www.isDone || time > 2))
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        if (www.isDone)
        {
            string output = www.text;
            string[] outputArray = output.Split(new char[] { '=', '&' });
            if (outputArray.Length > 1)
            {
                Vector2 position = new Vector2();
                float.TryParse(outputArray[1], out position.x);
                float.TryParse(outputArray[3], out position.y);
                position.x -= minPanPosition;
                position.x *= degreesToPanMultiplier;
                position.y -= minTiltPosition;
                position.y *= degreesToTiltMultiplier;
                //print(position.x + "     " + position.y);
                currentPosition = position;
            }
            else
            {
                currentPosition = GeneralMethods.WrongReturnAngle;
                StartCoroutine(GetCurrentPositionCommand());
            }
        }
        else
        {
            currentPosition = GeneralMethods.WrongReturnAngle;
            StartCoroutine(GetCurrentPositionCommand());
        }
    }


    /// <summary>
    /// Sends a command to the camera.
    /// </summary>
    /// <param name="command">A string of what command it should be, only the command, not the url.</param>
    /// <returns>the www which might cwontain a response</returns>
    private IEnumerator SendCommand(string command)
    {
        WWW www = new WWW(url + command);
        ++messagesSent;
        bool succes = false;
        int attempts = 0;
        while (attempts < maxAttemptsCommand && !succes)
        {
            float time = 0;
            attempts++;
            while (!(www.isDone || time > 2))
            {
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }
            if (www.isDone)
            {
                succes = true;
            }
        }
        if (!succes)
        {
            cameraState = State.unreachable;
            yield return false;
        }
        else {
            yield return www;
        }
    }
}