using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class ScoresController : MonoBehaviour
{
    
    private readonly string _webUrl = "http://localhost:5000/api/";

    public GameObject levelButtonTemplate;
    public GameObject recordPanelTemplate;
    public GameObject levelContent;
    public GameObject recordContent;

    private static List<Level> Levels;

    public void OnScoresOpen()
    {
        RefreshLevelTOContent();
    }

    public void RefreshLevelTOContent()
    {
        RefreshLevels();
    }

    public void RefreshLevels()
    {
        StartCoroutine(ApiController.Instance.Get<List<Level>>(_webUrl + "level", SetLevels, true));
    }

    private void SetLevels(List<Level> result)
    {
        Levels = result;
        
        foreach (Transform child in levelContent.transform)
        {
            Destroy(child.gameObject);
        }
        
        Debug.Log(result.ToString());
        foreach (Level level in Levels)
        {
            var copy = Instantiate(levelButtonTemplate, levelContent.transform);
            copy.GetComponentInChildren<TMP_Text>().text = level.levelName;

            copy.GetComponent<Button>().onClick.AddListener(
                () => {
                    RefreshRecords(level);
                });
        }
    }

    private void RefreshRecords(Level level)
    {                   
        Debug.Log("Pressed");
        foreach (Transform child in recordContent.transform)
        {
            Destroy(child.gameObject);
        }
        
        level.records.Sort((r1,r2) => r1.time.CompareTo(r2.time));
        
        Debug.Log("Pressed " + level);
        foreach (Record record in level.records)
        {
            var copy = Instantiate(recordPanelTemplate, recordContent.transform);

            copy.GetComponentsInChildren<TMP_Text>()[0].text = record.nickname;
            copy.GetComponentsInChildren<TMP_Text>()[1].text = TimeSpan.FromMilliseconds(record.time).ToString();

            if (record.nickname != LoginController._user.nickname)
            {
                continue;
            }
            
            var image = copy.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(255, 180, 0, 255);
            }
        }
    }
    
    [Serializable]
    public class Level
    {
        public string levelName;
        public List<Record> records;

        public override string ToString()
        {
            return levelName + " " + records.ToString();
        }
    }

    [Serializable]
    public class Record
    {
        public string nickname;
        public long time;

        public override string ToString()
        {
            return nickname + " " + time;
        }
    }
}
