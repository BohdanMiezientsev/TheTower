using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class ApiController : MonoBehaviour
{
    private static ApiController _instance;
    public static string WebUrl { get; private set; } = "http://localhost:5000/api/";

    public static ApiController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ApiController>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = nameof(ApiController);
                    _instance = go.AddComponent<ApiController>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

    public IEnumerator Get<T>(string url, System.Action<T> callback, bool anonymous = false)
    {
        using (var www = UnityWebRequest.Get(url))
        {
            if (!anonymous)
            {
                var user = FileWriter.ReadFromBinaryFile<LoginController.User>(
                    Application.dataPath + "/playerPref.txt");
                if (user != null)
                {
                    www.SetRequestHeader("Authorization", "Bearer " + user.validation);
                }
            }

            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ShowError(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log(jsonResult);
                    var result = JsonConvert.DeserializeObject<T>(jsonResult);
                    callback(result);
                }
            }
        }
    }
    
    public IEnumerator Post<T>(string url, string postData, System.Action<T> callback, bool anonymous = false)
    {
        using (var www = UnityWebRequest.Post(url, postData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            if (postData != null)
            {
                 www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(postData));
            }

            if (!anonymous)
            {
                var user = FileWriter.ReadFromBinaryFile<LoginController.User>(
                    Application.dataPath + "/playerPref.txt");
                if (user != null)
                {
                    www.SetRequestHeader("Authorization", "Bearer " + user.validation);
                }
            }
            
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                ShowError(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var jsonResult = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
                    Debug.Log($"URL: {url} \n response: {jsonResult} \n code: {www.responseCode}");
                    var result = JsonConvert.DeserializeObject<T>(jsonResult);
                    callback(result);
                }
            }
        }
    }
    
    private void ShowError(string text)
    {
        Debug.Log($"error: {text}");
        var errorScreen = ErrorMessageScreen.Instance;
        if (errorScreen != null)
        {
            errorScreen.ShowErrorMessage(text);
        } 
    }
}
