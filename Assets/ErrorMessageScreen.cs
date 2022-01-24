using TMPro;
using UnityEngine;

public class ErrorMessageScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private GameObject _panel;

    public static ErrorMessageScreen Instance;

    private void Awake()
    {
        Instance = this;
        _panel.SetActive(false);
    }

    public void ShowErrorMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        _text.text = text;
        _panel.SetActive(true);
    }
}
