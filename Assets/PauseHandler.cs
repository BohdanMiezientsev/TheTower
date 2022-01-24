using System.Collections;
using System.Collections.Generic;
using LevelScripts;
using MovementControl;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenu;

    private bool _isEscapePressed = false;
    private bool _finished = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !_isEscapePressed && !LevelManager.LevelFinished)
        {
            _finished = false;
            _isEscapePressed = true;
            var isViewOpened = _pauseMenu.activeSelf;
            _pauseMenu.SetActive(!isViewOpened);
            Time.timeScale = isViewOpened ? 1 : 0;
            var timer = TimerUI.Instance;

            if (timer == null)
            {
                return;
            }

            if (isViewOpened)
            {
                timer.StartTimer(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                MovementManager.IsLookUnlocked = true;
            }
            else
            {
                timer.StopTimer();
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                MovementManager.IsLookUnlocked = false;
            }

        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            _isEscapePressed = false;
        } 
        else if (LevelManager.LevelFinished && !_finished)
        {
            _pauseMenu.SetActive(true);
            Time.timeScale = 0;
            TimerUI.Instance.StopTimer();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            MovementManager.IsLookUnlocked = false;
            _finished = true;
        }
    }
}
