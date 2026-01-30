using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance;

    [Header("Datos del Usuario")]
    public string token;
    public int userId;
    public string username;
    public string email;
    public string tessitura;

    [Header("Estado")]
    public bool IsLoggedIn = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetSession(UserLoginResponse data)
    {
        token = data.access_token;
        userId = data.id;
        username = data.username;
        email = data.email;
        tessitura = data.tessitura;
        IsLoggedIn = true;

        Debug.Log($"<color=green>[UserSession] Sesión iniciada para: {username}</color>");
    }

    public void Logout()
    {
        token = null;
        userId = 0;
        username = null;
        IsLoggedIn = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}