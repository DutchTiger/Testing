using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script controlls a camera.
/// </summary>
public class CameraControllScript : MonoBehaviour {

    private float rotationA, rotationB; //How far the camera is rotated in both directions
    protected int xMultiplier, yMultiplier; //Current speed multipliers enabled in both directions.
    private float xSpeed; //Current actual rotation speed in degrees per second in the horizontal plane
    private float xBaseSpeed; //The horizontal speed at 1x multiplier.
    private float ySpeed; //Current actual rotation speed in degrees per second in the vertical plane.
    private float yBaseSpeed; //The vertical speed at 1x multiplier
    protected bool up, down, left, right; //What directions the camera currently should be moving in
    public double[] knownXRotationSpeeds; //An array of known rotation speeds of the camera in the horizontal plane
    public double[] knownYRotationSpeeds; //An array of known rotation speeds of the camera in the vertical plane
    protected bool anyChangesMade; //Whether or not any changes have been made in direction or speed, and whether to send to the camera.
    private float currentZoomLevel; //How far zoomed in the camera is.
    public GyroToControllsScript controller; //What is controlling the camera.
    public Vector2 currentPosition; 
    public bool currentPositionAccurate = false;
    public enum State { active, unreachable, locked};
    [HideInInspector]
    public State cameraState = State.active;

    private IEnumerator controllTimer;

    /// <summary>
    /// Set the base speeds and speeds.
    /// </summary>
    protected virtual  void Start()
    {
        knownXRotationSpeeds = new double[] { 0.3125, 0.625, 1.25, 2.5, 5, 10, 20, 40, 80, 160, 320 };
        knownYRotationSpeeds = new double[] { 0.15625, 0.3125, 0.625, 1.25, 2.5, 5, 10, 20, 40, 80, 160 };
        rotationA = 0;
        rotationB = 0;
        xBaseSpeed = 2;
        xSpeed = 0.2f * yBaseSpeed;
        yBaseSpeed = 2;
        ySpeed = 0.1f * yBaseSpeed;
        anyChangesMade = true;
        StartCoroutine(GetCurrentPositionCommand());
        CanBeControlled = true;


    }

    /// <summary>
    /// Set the directions
    /// </summary>
    protected virtual void SetDirections()
    {

        if (up)
        {
            TurnUp();
        }
        if (down)
        {
            TurnDown();
        }
        if (left)
        {
            TurnLeft();
        }
        if (right)
        {
            TurnRight();
        }
    }

    public virtual void Update()
    {
        
    }
    /// <summary>
    /// If any changes are made, set the directions
    /// </summary>
    public virtual void FixedUpdate()
    {
        if (anyChangesMade)
        {
            currentPositionAccurate = false;
            SetDirections();
        }
        anyChangesMade = false;
        if (rotationB > 90)
        {
            rotationB = 90;
        }
        if (rotationB < -90)
        {
            rotationB = -90;
        }
    }

    /// <summary>
    /// Send the camera an go-up command
    /// </summary>
    protected virtual void TurnUp()
    {
        rotationB -= ySpeed;
        UpdateAngles();
    }

    /// <summary>
    /// Send the camera a go-down command
    /// </summary>
    protected virtual void TurnDown()
    {
        rotationB += ySpeed;
        UpdateAngles();
    }

    /// <summary>
    /// Send the camera a go left command
    /// </summary>
    protected virtual void TurnLeft()
    {
        rotationA -= xSpeed;
        UpdateAngles();
    }

    /// <summary>
    /// Send the camera a go right command
    /// </summary>
    protected virtual void TurnRight()
    {
        rotationA += xSpeed;
        UpdateAngles();
    }


    /// <summary>
    /// Set ths horizontal speed of the camera
    /// </summary>
    /// <param name="speedMode">What speed the camera should have, -5 to 5.</param>
    public virtual void SetXSpeed(int speedMode)
    {
        if (speedMode < -5 || speedMode > 5)
        {
            return;
        }
        if (xMultiplier != speedMode)
        {
            xMultiplier = speedMode;
            xSpeed = 0.2f * Mathf.Pow(xBaseSpeed, speedMode);

            anyChangesMade = true;
        }
    }

    /// <summary>
    /// Set the vetical speed of the camera
    /// </summary>
    /// <param name="speedMode">What speed the camera should have, -5 to 5.</param>
    public virtual void SetYSpeed(int speedMode)
    {
        if (speedMode < -5 || speedMode > 5)
        {
            return;
        }
        if (yMultiplier != speedMode)
        {
            yMultiplier = speedMode;
            ySpeed = 0.1f * Mathf.Pow(yBaseSpeed, speedMode);

            anyChangesMade = true;
        }
    }

    /// <summary>
    /// Update the angles of the virtual camera
    /// </summary>
    private void UpdateAngles()
    {
        transform.rotation = Quaternion.Euler(new Vector3(rotationB, rotationA, 0));
    }

    /// <summary>
    /// Control the vertical camera directly
    /// </summary>
    /// <param name="x">The x angle</param>
    /// <param name="y">The y angle</param>
    /// <param name="z">The z angle</param>
    public void DirectControl(float x, float y, float z)
    {
        rotationB += y;
        rotationA += x;
        transform.rotation = Quaternion.Euler(new Vector3(rotationB, rotationA, 0));
    }

    /// <summary>
    /// Move the camera to a location, returns once it's done.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public virtual IEnumerator MoveToLocation(float x, float y)
    {
        rotationB += y;
        rotationA += x;
        transform.rotation = Quaternion.Euler(new Vector3(rotationB, rotationA, 0));
        yield return null;
    }

    /// <summary>
    /// Return the current rotation/position
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator GetCurrentPositionCommand()
    {
        currentPosition = new Vector2(rotationA, rotationB);
        currentPositionAccurate = true;
        yield return new Vector2(rotationA, rotationB);
    }


    public virtual IEnumerator GetCurrentZoomCommand()
    {
        yield return currentZoomLevel;
    }

    public bool Up
    {
        get
        {
            return up;
        }
        set
        {
            if (up != value)
            {
                anyChangesMade = true;
                up = value;
            }
        }
    }
    public bool Down
    {
        get
        {
            return down;
        }
        set
        {
            if (down != value)
            {
                anyChangesMade = true;
                down = value;
            }
        }
    }
    public bool Left
    {
        get
        {
            return left;
        }
        set
        {
            if (left != value)
            {
                anyChangesMade = true;
                left = value;
            }
        }
    }
    public bool Right
    {
        get
        {
            return right;
        }
        set
        {
            if (right != value)
            {
                anyChangesMade = true;
                right = value;
            }
        }
    }

    public bool CanBeControlled
    {
        get
        {
            //print(cameraState);
            if (cameraState == State.active)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
        set
        {
            if (value)
            {
                cameraState = State.active;
            }
            else
            {
                cameraState = State.locked;
            }
            if (cameraState == State.locked)
            {
                if (controllTimer == null)
                {
                    controllTimer = ControllTimer();
                    StartCoroutine(controllTimer);
                }
            }
        }
    }

    private IEnumerator ControllTimer()
    {
        yield return new WaitForSeconds(2);
        CanBeControlled = true;
        controllTimer = null;
    }
}