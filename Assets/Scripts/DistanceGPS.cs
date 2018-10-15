using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceGPS : MonoBehaviour {

    float earthRadius = 6378.1370f;
    float _d2r = (Mathf.PI / 180f);

    float currentLat = 52.371901f;
    float currentLong = 4.909680f;

    float shipLat = 52.371727f;
    float shipLong = 4.914853f;

    private void Start()
    {
        Debug.Log("Distance = " + HaversineInM(currentLat, currentLong, shipLat, shipLong));
    }

    //Formule to calculate the distance between 2 GPS Positions
    public int HaversineInM(float lat1, float long1, float lat2, float long2)
    {
        return (int)(1000f * HaversineInKM(lat1, long1, lat2, long2));
    }

    private float HaversineInKM(float lat1, float long1, float lat2, float long2)
    {
        float dlong = (long2 - long1) * _d2r;
        float dlat = (lat2 - lat1) * _d2r;
        float a = Mathf.Pow(Mathf.Sin(dlat / 2f), 2f) + Mathf.Cos(lat1 * _d2r) * Mathf.Cos(lat2 * _d2r) * Mathf.Pow(Mathf.Sin(dlong / 2f), 2f);
        float c = 2f * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1f - a));
        float d = earthRadius * c;

        return d;
    }
}
