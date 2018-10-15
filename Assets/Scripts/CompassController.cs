using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CompassController : MonoBehaviour
{
    public Text compassText;
    public GameObject compassNeedle;
    public bool isTrueHeading = false;

    int averageFrames = 10; //Amount of frames that average is calculated over.
    int framesPassed = 0; // Amount of frames passed.
    float[] heading = new float[10]; // Array with last X heading values.
    float averageAngle;

    // Use this for initialization
    void Start()
    {
        Input.compass.enabled = true;
        Input.compensateSensors = true; //Enables tilt compensation provided by Unity
        Input.location.Start(); //Need to get the true heading
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCompass();
    }

    /// <summary>
    /// Updates rotation of the compass needle according to the compass heading 
    /// </summary>
    void UpdateCompass()
    {
        //Calculate average over X amount of frames to get rid of noise. 
        // !!! Better way is to filter the raw sensor data. Only this doesn't have tilt compensation implemented, this has to be done by hand. !!!
        if (framesPassed > averageFrames - 1)
        {
            averageAngle = MeanAngle(heading);

            framesPassed = 0;
        }

        //Get the heading from the compass
        if (isTrueHeading)
        {
            heading[framesPassed] = Input.compass.trueHeading;
        }
        else
        {
            heading[framesPassed] = Input.compass.magneticHeading;
        }

        //Rotate the needle along the y-axis to the heading angle.
        Quaternion rot = Quaternion.Euler(0, averageAngle, 0);
        compassNeedle.transform.rotation = Quaternion.Slerp(compassNeedle.transform.rotation, rot, 5 * Time.deltaTime);

        compassText.text = "Compass: " + Mathf.RoundToInt(averageAngle).ToString("0.##") + " °";

        framesPassed++;
    }

    /// <summary>
    /// Gives the average of an array with circular angles
    /// https://rosettacode.org/wiki/Averages/Mean_angle#C.23
    /// </summary>
    /// <returns>The average of the circular angles array</returns>
    /// <param name="angles">The array with circular angles</param>
    float MeanAngle(float[] angles)
    {
        var x = angles.Sum(a => Mathf.Cos(a * Mathf.PI / 180f)) / angles.Length;
        var y = angles.Sum(a => Mathf.Sin(a * Mathf.PI / 180f)) / angles.Length;

        float average = Mathf.Atan2(y, x) * 180f / Mathf.PI;
        return (average + 360) % 360; //Convert -180 - 180 to 0 - 359
    }
}
