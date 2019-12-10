using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GetWeatherInfo))]
public class GetLocation : MonoBehaviour
{

    GetWeatherInfo m_weatherScript;

    void Awake()
    {
        m_weatherScript = this.GetComponent<GetWeatherInfo>();
        m_weatherScript.enabled = false;

    }

    IEnumerator Start()
    {
#if UNITY_STANDALONE
        Debug.Log("Location is not enabled, using default location.");
        m_weatherScript.enabled = true;
        yield break;
#endif
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");

            yield break;
        }
        else
        {
            m_weatherScript.m_lon = Input.location.lastData.longitude;
            m_weatherScript.m_lat = Input.location.lastData.latitude;
            m_weatherScript.enabled = true;
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }
}
