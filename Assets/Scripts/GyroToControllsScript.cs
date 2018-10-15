using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Convert the output of a gyroscope, to the input of a camera that only has quad-directional input at set speeds
/// </summary>
public class GyroToControllsScript : MonoBehaviour
{
    public bool gyroDataNew = true;

    public CameraControllScript cc; //The camera
    public GyroScript gyro; //The gyroscope
    Vector3 gyroData; //The data gathered from the gyroscope

    public enum ControlMode { QuadDirectional, Positional, Direct }
    public ControlMode mode;
    bool controlling = false;

    float threshhold = 10; //Minimum change that needs to be made.

    float gyroAngleX = 0; //The accumilative rotation of the gyroscope
    float gyroAngleY = 0;

    float camAngleX = 0; //The accumilative rotation of the camera
    float camAngleY = 0;

    float camAngleXTrue = 0; //The true angle of the camera.
    float camAngleYTrue = 0;

    float errorX = 0; //The current difference between the two devices.
    float errorY = 0;

    //===============================================
    //For Quaddirectional
    float smallestXDelta, smallestYDelta; //Smallest difference between the required speed, and the closests available set speed.
    int smallestXDeltaIndex, smallestYDeltaIndex; //The index at which that smallests difference was, what set speed was closetst

    float normalThreshold = 10;  //Normal threshold
    float thresholdForTheThreshhold = 1; //Minimum the camera needs to get close to the phone's camera, before the normal threshold applies again.


    //================================================
    //For Positional
    bool cameraMoving;
    float gyroAngleXAtSend;
    float gyroAngleYAtSend;
    
    /// <summary>
    /// Let the phone never turn off.
    /// </summary>
    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        cc.controller = this;

        StartCoroutine(GetStartPosition());
    }

    /// <summary>
    /// Get the start position
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetStartPosition()
    {
        Vector2 position = new Vector2();
        StartCoroutine(cc.GetCurrentPositionCommand());
        yield return new WaitUntil(() => cc.currentPosition != new Vector2());
        position = cc.currentPosition;
        if (position == GeneralMethods.WrongReturnAngle)
        {
            cc.currentPosition = new Vector2();
            StartCoroutine(GetStartPosition());
            yield break;
        }
        cc.currentPosition = new Vector2();
        GyroAngleX = position.x;
        GyroAngleY = position.y;
        CamAngleX = position.x;
        CamAngleY = position.y;
        CamAngleXTrue = position.x;
        CamAngleYTrue = position.y;
        transform.rotation = Quaternion.Euler(0/*-CamAngleY*/, CamAngleX, 0);
        controlling = true;
        if (mode == ControlMode.Positional)
        {
            StartCoroutine(GetCurrentPosition());
        }
    }


    /// <summary>
    /// Get the current position 
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetCurrentPosition()
    {
        Vector2 position;
        StartCoroutine(cc.GetCurrentPositionCommand());
        yield return new WaitUntil(() => cc.currentPosition != new Vector2() && cc.currentPosition != GeneralMethods.WrongReturnAngle);
        position = cc.currentPosition;
        CamAngleX = position.x;
        CamAngleY = position.y;
        CamAngleXTrue = position.x;
        CamAngleYTrue = position.y;
        cc.currentPosition = new Vector2();

        StartCoroutine(GetCurrentPosition());
    }


    /// <summary>
    /// Get new data from the gyroscope, process it, and send to the camera
    /// </summary>
    public void FixedUpdate()
    {
        gyroDataNew = false;
        gyroData = gyro.GetData(); //Difference in rotation in degrees since last step.
        if (gyro.HasNewData)
        {
            gyroDataNew = true;
            GyroAngleX += 2 * gyroData.x; //Add the difference of the last frame.
            GyroAngleY += 2 * gyroData.y;
        }
        float errorX1 = camAngleX - gyroAngleX;
        float errorX2 = camAngleX - (360f - gyroAngleX);
        errorX = errorX2;
        if (Mathf.Abs(errorX1) < Mathf.Abs(errorX2))
        {
            errorX = errorX1;
        }
        ErrorY = camAngleY - gyroAngleY;
        
        if (controlling && cc.CanBeControlled)
        {
            if (mode == ControlMode.QuadDirectional)
            {
                QuadDirectionalControlStep();
            }
            else if (mode == ControlMode.Direct)
            {
                DirectControlStep();
            }
            else if (mode == ControlMode.Positional)
            {
                PositionalControlStep();
            }
        }
    }


    /// <summary>
    /// This is executes a step in quaddirectional mode.
    /// </summary>
    private void QuadDirectionalControlStep()
    {
        if (errorX < 0 && Mathf.Abs(ErrorX) > threshhold) //If the difference between what the camera should be, and what it actually is, is bigger than the minimum threshold for updating.
        {
            cc.Right = true;
            cc.Left = false;
            threshhold = thresholdForTheThreshhold; //Once it starts moving, make it aim for more prescession.
            StartCoroutine(GetCurrentPosition());
        }
        else if (errorX > 0 && Mathf.Abs(ErrorX) > threshhold) //Same, only for going left.
        {
            cc.Right = false;
            cc.Left = true;
            threshhold = thresholdForTheThreshhold;
            StartCoroutine(GetCurrentPosition());
        }
        else //Else don't move.
        {
            cc.Right = false;
            cc.Left = false;
            threshhold = normalThreshold;
        }

        if (errorY < 0 && Mathf.Abs(ErrorY) > threshhold) //Same but with Y.
        {
            cc.Up = true;
            cc.Down = false;
            threshhold = thresholdForTheThreshhold;
            StartCoroutine(GetCurrentPosition());
        }
        else if (errorY > 0 && Mathf.Abs(ErrorY) > threshhold)
        {
            cc.Up = false;
            cc.Down = true;
            threshhold = thresholdForTheThreshhold;
            StartCoroutine(GetCurrentPosition());
        }
        else
        {
            cc.Up = false;
            cc.Down = false;
            threshhold = normalThreshold;
        }

        smallestXDelta = float.MaxValue;
        smallestXDeltaIndex = 0;

        if (Mathf.Abs(ErrorX) > threshhold) //If the difference (error) is bigger than threshold, seek the speed closest to what's needed.
        {
            for (int i = 0; i < cc.knownXRotationSpeeds.Length; i++)
            {
                if (Mathf.Abs(Mathf.Abs(errorX) - ((float)cc.knownXRotationSpeeds[i] * Time.fixedDeltaTime)) < smallestXDelta) //If unsigned error - known rotationspeeds expressed in degrees per step, rather than per second, is smaller than smallestdelta
                {
                    smallestXDelta = Mathf.Abs(Mathf.Abs(errorX) - ((float)cc.knownXRotationSpeeds[i] * Time.fixedDeltaTime)); //then this known rotation speed is closer to how much needs to be rotated.
                    smallestXDeltaIndex = i;
                }
            }

            cc.SetXSpeed(smallestXDeltaIndex - 5); //-5 because the camera has settings -5, -4.. 4, 5, rather than starting at 0.
            CamAngleX += -Mathf.Sign(errorX) * (float)cc.knownXRotationSpeeds[smallestXDeltaIndex] * Time.fixedDeltaTime;

        }


        smallestYDelta = float.MaxValue;
        smallestYDeltaIndex = 0;

        if (Mathf.Abs(ErrorY) > threshhold)
        {
            for (int i = 0; i < cc.knownYRotationSpeeds.Length; i++)
            {
                if (Mathf.Abs(Mathf.Abs(errorY) - ((float)cc.knownYRotationSpeeds[i] * Time.fixedDeltaTime)) < smallestYDelta)
                {
                    smallestYDelta = Mathf.Abs(Mathf.Abs(errorY) - ((float)cc.knownYRotationSpeeds[i] * Time.fixedDeltaTime));
                    smallestYDeltaIndex = i;
                }
            }

            cc.SetYSpeed(smallestYDeltaIndex - 5);//-5 because the camera has settings -5, -4.. 4, 5, rather than starting at 0.
            CamAngleY += -Mathf.Sign(errorY) * (float)cc.knownYRotationSpeeds[smallestYDeltaIndex] * Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// This exectues a step in direct mode.
    /// </summary>
    private void DirectControlStep()
    {
        Vector3 gyroData = gyro.GetData();
        cc.DirectControl(gyroData.x, -gyroData.y, 0);
    }

    /// <summary>
    /// This executes a step in positional mode.
    /// </summary>
    private void PositionalControlStep()
    {
        if (!((Mathf.Abs(ErrorX) > threshhold || Mathf.Abs(ErrorY) > threshhold) && !cameraMoving))
        {
            return;
        }
        cameraMoving = true;
        GyroAngleX = GeneralMethods.OverFlowAngle(gyroAngleX);
        gyroAngleXAtSend = GyroAngleX;
        gyroAngleYAtSend = GyroAngleY;
        StartCoroutine(cc.MoveToLocation(gyroAngleX, gyroAngleY));
        

    }


    /// <summary>
    /// Gives a speed multiplicationfactor based on zoom level.
    /// </summary>
    /// <param name="fullZoom">How many times zoomed in is full zoom</param>
    /// <param name="currentZoom">What is the current zoom</param>
    /// <param name="fullzoomspeedfactor">The number by which to multiply the speed at full zoom.</param>
    /// <returns></returns>
    private float ZoomFactorSpeed(float fullZoom, float currentZoom, float fullzoomspeedfactor)
    {
        return (1 - ((1 - ((fullZoom - currentZoom) / fullZoom)) * (1 - fullzoomspeedfactor)));
    }
    

    /// <summary>
    /// Change seconds intop gamesteps.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private Vector3 StepDataToSeconds(Vector3 input)
    {
        return input * 50;
    }


    /// <summary>
    /// To be called once the camera's done moving, and ready to receive a new command
    /// </summary>
    public void PositionalCameraDoneMoving()
    {
        cameraMoving = false;
        CamAngleX = gyroAngleXAtSend;
        CamAngleY = gyroAngleYAtSend;
        CamAngleXTrue = gyroAngleXAtSend;
        CamAngleYTrue = gyroAngleYAtSend;
    }



    public float GyroAngleX
    {
        get
        {
            return gyroAngleX;
        }
        private set
        {
            gyroAngleX = value;
            gyroAngleX = GeneralMethods.OverFlowAngle(gyroAngleX);
        }
    }
    public float GyroAngleY
    {
        get
        {
            return gyroAngleY;
        }
        private set
        {
            gyroAngleY = Mathf.Clamp(value, 0, 90);
        }
    }


    public float CamAngleX
    {
        get
        {
            return camAngleX;
        }
        private set
        {
            camAngleX = value;
            camAngleX = GeneralMethods.OverFlowAngle(camAngleX);
        }
    }
    public float CamAngleY
    {
        get
        {
            return camAngleY;
        }
        private set
        {
            camAngleY = value;
        }
    }

    public float CamAngleXTrue
    {
        get
        {
            return camAngleXTrue;
        }
        private set
        {
            camAngleXTrue = value;
            camAngleXTrue = GeneralMethods.OverFlowAngle(camAngleXTrue);
        }
    }
    public float CamAngleYTrue
    {
        get
        {
            return camAngleYTrue;
        }
        private set
        {
            camAngleYTrue = value;
        }
    }

    private float ErrorX
    {
        get
        {
            return errorX;
        }
        set
        {
            errorX = value;
        }
    }
    private float ErrorY
    {
        get
        {
            return errorY;
        }
        set
        {
            errorY = value;
        }
    }
}