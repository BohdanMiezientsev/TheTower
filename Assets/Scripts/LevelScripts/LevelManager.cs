using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LevelScripts
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private List<LevelTrigger> levelTriggers;

        private TimerUI _timer;
        public static bool LevelFinished = false;

        private void Awake()
        {
            foreach (var levelTrigger in levelTriggers)
            {
                levelTrigger.SetLevelManager(this);
            }
        }

        private void Start()
        {
            _timer = TimerUI.Instance;
            _timer.CleanTimer();
            LevelFinished = false;
        }

        public void TriggerWorked(LevelTriggerType type)
        {
            switch (type)
            {
                case LevelTriggerType.Start:
                {
                    StartLevel();
                    return;
                }
                case LevelTriggerType.Finish:
                {
                    FinishLevel();
                    return;
                }
                default: return;
            }
        }

        private void StartLevel()
        {
            _timer.StartTimer();
        }

        private void FinishLevel()
        {
            _timer.StopTimer();
            LevelFinished = true;
            var user = FileWriter.ReadFromBinaryFile<LoginController.User>(Application.dataPath + "/playerPref.txt");
            if (user?.validation != null)
            {
                StartCoroutine(ApiController.Instance.Post<Record>(
                    ApiController.WebUrl + "record/"+ SceneManager.GetActiveScene().name + "/" +
                    _timer.TotalTimeInMilliseconds,
                    null, Callback));
            }
        }

        private void Callback(Record record)
        {
            if (record != null)
            {
                
            }
        }
        
        [Serializable]
        public class Record
        {
            public string Nickname { get; set; }
            [JsonIgnore]
            public LoginController.User User { get; set; }
            public string LevelName { get; set; }
            [JsonIgnore]
            public ScoresController.Level Level { get; set; }
            public long Time { get; set; }
        
        }
    }
}
