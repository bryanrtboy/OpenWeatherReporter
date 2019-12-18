using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveInitialData : MonoBehaviour
{
    public TextAsset m_backupWeatherJson;
    int count;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Count"))
        {
            Destroy(this);
        }
        else
        {
            //First time running, create 10 copies...
            for (int i = 0; i < 10; i++)
            {
                Debug.Log("saving...", this);
                var bytes = System.Text.Encoding.UTF8.GetBytes(m_backupWeatherJson.text);
                Save("Date" + i.ToString() + ".json", bytes);
            }
        }
    }

    /// <summary>
    /// Test - writing and reading a text file. Should work with any byte stream (like the one from Texture2D.EncodeToPNG)
    /// </summary>
    void Start()
    {
        Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath, this);


        //Debug.Log("saving...", this);
        //var bytes = System.Text.Encoding.UTF8.GetBytes("Test for count " + count.ToString());
        //Save("test" + count.ToString() + ".txt", bytes);
        //Debug.Log("Saved " + count.ToString() + ".txt");
        //count++;
        //if (count > 10)
        //    count = 0;
        //PlayerPrefs.SetInt("Count", count);


        Debug.Log("loading...", this);
        var loadedBytes = Load("Date" + UnityEngine.Random.Range(0, 10).ToString() + ".json");
        var str = System.Text.Encoding.UTF8.GetString(loadedBytes);
        Debug.Log("got string: " + str, this);
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
