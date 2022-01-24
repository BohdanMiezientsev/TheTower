using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

public class LoginController : MonoBehaviour
{
    [Header("Login fields")]
    [SerializeField] private TMP_InputField _nicknameField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_Text _userText;
    [Header("Register fields")]
    [SerializeField] private TMP_InputField _nicknameRegisterField;
    [SerializeField] private TMP_InputField _emailRegisterField;
    [SerializeField] private TMP_InputField _passwordRegisterField;
    [SerializeField] private Button _registerButton;
    
    public static User _user;
    private string _tempPassword = string.Empty;

    private void Awake()
    {
        _registerButton.onClick.AddListener(Register);
    }

    private void Start()
    {
        User user = FileWriter.ReadFromBinaryFile<User>(Application.dataPath + "/playerPref.txt");
        if (user?.nickname != null && !user.nickname.Equals(""))
        {
            Debug.Log($"user data from file on start: \n {user}");
            _user = user;
            _userText.text = _user.nickname;
            Login(true);
        }
    }

    private void OnDestroy()
    {
        _registerButton.onClick.RemoveListener(Register);
    }

    public void Login(bool fromUserFile = false)
    {
        if (!fromUserFile)
        {
            User temp = new User {nickname = _nicknameField.text, password = _passwordField.text};
            if (_user == null || (temp.nickname != "" && !_user.nickname.Equals(temp.nickname)))
            {
                _user = temp;
            }
            
            SendLoginRequest();
        }
    }

    private void SendLoginRequest()
    {
        var jsonString = JsonUtility.ToJson(_user);
        StartCoroutine(ApiController.Instance.Post<ValidationToken>(ApiController.WebUrl + "user/login", jsonString, ValidationTokenCallback, true));
    }

    private void ValidationTokenCallback(ValidationToken result)
    {
        if (result?.token != null)
        {
            Debug.Log("validation token callback" + result.token);
            _user.validation = result;
            _userText.text =  _user.nickname;
            FileWriter.WriteToBinaryFile(Application.dataPath+"/playerPref.txt", _user);
        }
    }

    private void Register()
    {
        var registerRequest = new RegisterRequest()
        {
            nickname = _nicknameRegisterField.text,
            email = _emailRegisterField.text,
            password = _passwordRegisterField.text
        };

        _tempPassword = registerRequest.password;

        var jsonString = JsonUtility.ToJson(registerRequest);
        StartCoroutine(ApiController.Instance.Post<RegisterCallbackClass>(ApiController.WebUrl + "user", jsonString, RegisterCallback, true));
    }

    private void RegisterCallback(RegisterCallbackClass callback)
    {
        _user.nickname = callback.nickname;
        _user.password = _tempPassword;
        _tempPassword = string.Empty;
        
        SendLoginRequest();
    }

    [Serializable]
    public class User
    {
        public string nickname;
        public string password;
        public ValidationToken validation;

        public override string ToString()
        {
            return nickname + " " + password + " " + validation.ToString();
        }
    }

    [Serializable]
    public class RegisterRequest
    {
        public string nickname;
        public string email;
        public string password;
    }

    [Serializable]
    public class RegisterCallbackClass
    {
        public string nickname;
        public string email;
        public int role;
    }

    [Serializable]
    public class ValidationToken
    {
        public string token;

        public override string ToString()
        {
            return token + "";
        }
    }
}