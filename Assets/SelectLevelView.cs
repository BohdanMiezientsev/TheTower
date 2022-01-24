using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectLevelView : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private Button _moveToSceneButton;
    private void Awake()
    {
        _moveToSceneButton.onClick.AddListener(LoadScene);
    }

    private void OnDestroy()
    {
        _moveToSceneButton.onClick.RemoveAllListeners();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(_sceneName);
    }
}
