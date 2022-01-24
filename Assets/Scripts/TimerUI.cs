using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TimerUI : MonoBehaviour
{
    public static TimerUI Instance { get; private set; }

    [FormerlySerializedAs("_text")] [SerializeField] private TMP_Text text;
    [SerializeField] private string _stringFormat = "{0:D2}:{1:D2}:{2:D2}";
    
    public long TotalTimeInMilliseconds => (long) (_totalTime * 1000); 
    private float _totalTime = 0;
    private Coroutine _timer;
    private bool _isCounting = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartTimer(bool forceStart = true)
    {
        if (_timer != null)
        {
            return;
        }

        if (forceStart || _isCounting)
        {
            _isCounting = true;
            _timer = StartCoroutine(TimerCoroutine());
        }

    }

    public void StopTimer()
    {
        if (_timer == null)
        {
            return;
        }
        
        StopCoroutine(_timer);
        _timer = null;
    }

    public void CleanTimer()
    {
        _totalTime = 0;
        SetText();
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            _totalTime += Time.deltaTime;
            SetText();
            yield return null;
        }
    }

    private void SetText()
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(TotalTimeInMilliseconds);
        string temp = string.Format(_stringFormat, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        text.text = temp;
    }
}
