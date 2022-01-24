using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseView : MonoBehaviour
{
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _restartButton;

    private const string _menuSceneName = "Menu";

    private void Start()
    {
        _menuButton.onClick.AddListener(LoadMenu);
        _restartButton.onClick.AddListener(LoadCurrentLevel);
    }

    private void OnDestroy()
    {
        _menuButton.onClick.RemoveAllListeners();
        _restartButton.onClick.RemoveAllListeners();
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(_menuSceneName);
    }

    private void LoadCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    

}
