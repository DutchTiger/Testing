using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOnPosition : MonoBehaviour {

    public float xPosition;
    public float zPosition;
    public GameObject lowPolyBoatPrefab;
    public GameObject lowPolyBoat;
    public GameObject shipIndicator3DPreFab;
    public string ShipOne;

    public float[] shipsLat;
    public float[] shipsLong;
    public string[] shipsName;
    public Material[] material;

    // Use this for initialization
    void Start () {
        //Fake AIS gps position and name of ships.
        shipsLat = new float[5];
        shipsLong = new float[5];
        shipsName = new string[5];

        shipsLat[0] = 52.373987f;
        shipsLong[0] = 4.910235f;
        shipsName[0] = "Red";

        shipsLat[1] = 52.366260f;
        shipsLong[1] = 4.903764f;
        shipsName[1] = "Green";

        shipsLat[2] = 52.373570f;
        shipsLong[2] = 4.903421f;
        shipsName[2] = "Pink";

        shipsLat[3] = 52.374094f;
        shipsLong[3] = 4.907283f;
        shipsName[3] = "Yellow";

        shipsLat[4] = 52.371579f;
        shipsLong[4] = 4.913807f;
        shipsName[4] = "Orange";

        ShipNameController.Initialize();

        //loop through the fake AIS arrays and place a ship in the world where the ship should be.
        for (int i = 0; i < shipsName.Length; i++)
        {
            ShipPosition(shipsLat[i], shipsLong[i]);
            //Debug.Log("x = " + xPosition + " z = " + zPosition);
            lowPolyBoat = Instantiate(lowPolyBoatPrefab, new Vector3(xPosition, 0, zPosition), Quaternion.identity);
            lowPolyBoat.GetComponent<MeshRenderer>().sharedMaterial = material[i];
            ShipNameController.CreateShipName(shipsName[i], this.transform, lowPolyBoat.transform);
            //Debug.Log("My transform = " + lowPolyBoat.transform.position);
        }
        
    }

    //Function to calculate the ships position from the user
    //with this function we can convert gps locations to the unity world scale
    void ShipPosition(float latitude, float longitude)
    {
        DistanceGPS gps = new DistanceGPS();

        //Current position could be received from the phone's GPS Sensor
        float currentLat = 52.371970f;
        float currentLong = 4.909706f;

        float shipLat = latitude;
        float shipLong = longitude;

        zPosition = gps.HaversineInM(currentLat, currentLong, shipLat, currentLong);
        xPosition = gps.HaversineInM(currentLat, currentLong, currentLat, shipLong);

        if (currentLat > shipLat)
        {
            zPosition = zPosition - 2 * zPosition;
        }
        if (currentLong > shipLong)
        {
            xPosition = xPosition - 2 * xPosition;
        }
        
    }
}
