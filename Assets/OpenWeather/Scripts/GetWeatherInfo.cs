using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class GetWeatherInfo : MonoBehaviour
{

    [Tooltip("The root url from openweathermap.org")]
    public string m_openMapAPIRootURL = "https://api.openweathermap.org/data/2.5/weather?";
    [Tooltip("Copy/paste your API key from your open weather account")]
    public string m_openMapAPIKey = "";
    [Tooltip("If you are not building to a mobile device, use a placeholder latitude")]
    public float m_lat = 39.7661f;
    [Tooltip("If you are not building to a mobile device, use a placeholder longitude")]
    public float m_lon = -105.077209f;
    [Tooltip("View some basic conditions in the game view here:")]
    public Text m_UI;

    public CurrentWeather m_currentConditions;


    string m_url = "";

    void OnEnable()
    {
        m_url = m_openMapAPIRootURL + "lat=" + m_lat.ToString() + "&lon=" + m_lon.ToString() + "&appid=" + m_openMapAPIKey;

        StartCoroutine(GetRequest(m_url));

    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                Load(webRequest.downloadHandler.text);

            }
        }
    }

    public void Load(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, m_currentConditions);

        DateTime dateTime = new DateTime();
        dateTime = dateTime.AddSeconds(m_currentConditions.dt);

        if (m_UI != null)
            m_UI.text = "The weather is " + m_currentConditions.weather[0].main + " in " + m_currentConditions.name;
    }

}

[Serializable]
public class CurrentWeather
{

    public Coordinates coord;
    public Weather[] weather;
    public Main main;
    public Wind wind;
    public Clouds clouds;
    [SerializeField]
    public int dt;
    public Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;
}

[Serializable]
public class Coordinates
{
    public float lon;
    public float lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main;
    public string description;
    public string icon;
}

[Serializable]
public class Main
{
    public float temp;
    public float pressure;
    public float humidity;
    public float temp_min;
    public float temp_max;

}

[Serializable]
public class Wind
{
    public float speed;
    public float deg;
}


[Serializable]
public class Clouds
{
    public int all;
}

public class Sys
{
    public int type;
    public int id;
    public float message;
    public string country;
    public int sunrise;
    public int sunset;
}

