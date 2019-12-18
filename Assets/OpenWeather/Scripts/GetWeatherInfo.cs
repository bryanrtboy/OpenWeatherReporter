using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;
using System.IO;

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
    int count;

    void OnEnable()
    {
        m_url = m_openMapAPIRootURL + "lat=" + m_lat.ToString() + "&lon=" + m_lon.ToString() + "&appid=" + m_openMapAPIKey;
        StartCoroutine(GetRequest(m_url));

        //Keep track of number of times we have saved weather data
        if (PlayerPrefs.HasKey("Count"))
            count = PlayerPrefs.GetInt("Count");
        else
            PlayerPrefs.SetInt("Count", count);
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

                //If there is no internet, use a backup session that we have saved from previous sessions
                string fileName = "Date" + UnityEngine.Random.Range(0, 10).ToString() + ".json";
                var loadedBytes = Load(fileName);
                var str = System.Text.Encoding.UTF8.GetString(loadedBytes);
                LoadUsingJson(str);
                Debug.Log("Loaded " + fileName + " in place of realtime weather");
                yield break;
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                SaveData(webRequest.downloadHandler.text);
                LoadUsingJson(webRequest.downloadHandler.text);

            }
        }
    }

    void SaveData(string savedData)
    {
        string dateValue = System.DateTime.Now.Day.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Year.ToString();
        string dateKey = "Date" + count.ToString();

        //Debug.Log("Date key is " + dateKey + ", Date value is " + dateValue);

        //The Key/Value pair is for example – "Date0", 18122019. This means only unique Values get saved for any given Key. One forecast
        //is thus saved per day, giving us 10 unique records in the backup database saved to disk. 
        if (PlayerPrefs.HasKey(dateKey))
        {
            if (PlayerPrefs.GetString(dateKey) == dateValue)
            {
                Debug.Log("Record already fetched for today, not saving again");
                return;
            }
            else
            {
                //Key exists, but was from a different day, so set this as a Key/Value pair and write forecast to disk
                PlayerPrefs.SetString(dateKey, dateValue);
                WriteCurrentDataToFile(savedData);
            }
        }
        else
        {
            //First ten plays will always write to disk
            PlayerPrefs.SetString(dateKey, dateValue);
            WriteCurrentDataToFile(savedData);
        }
    }

    void WriteCurrentDataToFile(string savedData)
    {
        count++;
        if (count > 10)
            count = 0;

        PlayerPrefs.SetInt("Count", count);
        var bytes = System.Text.Encoding.UTF8.GetBytes(savedData);
        Save("Date" + count.ToString() + ".json", bytes);
    }

    public void LoadUsingJson(string savedData)
    {
        JsonUtility.FromJsonOverwrite(savedData, m_currentConditions);

        DateTime dateTime = new DateTime();
        dateTime = dateTime.AddSeconds(m_currentConditions.dt);

        float temp = m_currentConditions.main.temp * 9 / 5 - 459.67f;

        if (m_UI != null)
            m_UI.text = dateTime.ToShortDateString() + ": It's " + temp.ToString("#.#") + " degrees F and " + m_currentConditions.weather[0].main + " in " + m_currentConditions.name;
    }

    public static void Save(string name, byte[] bytes)
    {
        var path = System.IO.Path.Combine(Application.persistentDataPath, name);
        System.IO.File.WriteAllBytes(path, bytes);
    }

    public static byte[] Load(string name)
    {
        var path = System.IO.Path.Combine(Application.persistentDataPath, name);
        return System.IO.File.ReadAllBytes(path);
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

