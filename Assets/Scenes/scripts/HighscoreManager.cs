using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SocialPlatforms;
using System.Linq;
using System;
using static System.Net.Mime.MediaTypeNames;

public class HighscoreManager : MonoBehaviour
{

    public static HighscoreManager Instance { get; private set; }


    public List<float> Highscores = new List<float>();

    public HighscoreDisplay highscoresDisplay;
    public SavedHighscores savedHighscores;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        //save highscores in /HighScores/
        if (!Directory.Exists(UnityEngine.Application.persistentDataPath + "/HighScores/"))
        {
            Directory.CreateDirectory(UnityEngine.Application.persistentDataPath + "/HighScores/");
        }
    }

    public void addHighscore(float entryScore)
    {
        Highscores.Add(entryScore);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Application starting, loading highscores path is" + (" " + UnityEngine.Application.persistentDataPath));
        if (Highscores == null || Highscores.Count == 0)
        {
            if (File.Exists(UnityEngine.Application.persistentDataPath + "/HighScores/highscores.xml"))
            {
                Debug.Log("highscore file exists");
                XmlSerializer serializer = new XmlSerializer(typeof(SavedHighscores));
                FileStream stream = new FileStream(UnityEngine.Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Open);
                savedHighscores = serializer.Deserialize(stream) as SavedHighscores;
                stream.Close();
                Highscores = savedHighscores.list;
            }
            else
            {
                Debug.Log("highscore didnt exists so create new ones");

                addHighscore(1234);
                addHighscore(1);
                addHighscore(7);
                addHighscore(13);

                savedHighscores.list = Highscores;
                XmlSerializer serializer = new XmlSerializer(typeof(SavedHighscores));
                FileStream stream = new FileStream(UnityEngine.Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
                serializer.Serialize(stream, savedHighscores);
                stream.Close();
            }
        }
        else
        {
            Debug.Log("Already have highscores so dont need to load");
        }
    }
    // Update is called once per frame
    void Update()
    {
        //check if the scene got reconstructed and rebuild the display
        if (!highscoresDisplay)
        {
            GameObject highscoresDisplayObject = GameObject.Find("HighscoreDisplay");
            if (highscoresDisplayObject != null)
            {
                highscoresDisplay = highscoresDisplayObject.GetComponent<HighscoreDisplay>();
            }
        }
        //check again if the loading worked
        if (highscoresDisplay)
        {
            Highscores.Sort((a, b) => b.CompareTo(a)); //descending order
            for (int i = 0; i < highscoresDisplay.Displays.Length; i++)
            {
                if (i < Highscores.Count)
                {
                    highscoresDisplay.Displays[i].text = "" + Highscores[i];
                }
                else
                {
                    highscoresDisplay.Displays[i].text = "";
                }
            }
        }
    }
    void OnApplicationQuit()
    {
        Debug.Log("Application ending, saving highscores:");
        savedHighscores.list = Highscores;
        XmlSerializer serializer = new XmlSerializer(typeof(SavedHighscores));
        FileStream stream = new FileStream(UnityEngine.Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
        serializer.Serialize(stream, savedHighscores);
        stream.Close();
    }
}
[System.Serializable]
public class SavedHighscores
{
    public List<float> list = new List<float>();
}
