using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    [Header("User Data")]
    public int id;
    public string username;
    public string email;
    public string tessitura;
    public string token;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Actualizado para incluir el Token
    public void SetUser(int id, string username, string email, string tessitura, string token)
    {
        this.id = id;
        this.username = username;
        this.email = email;
        this.tessitura = tessitura;
        this.token = token;
    }

    public void Clear()
    {
        id = 0;
        username = "";
        email = "";
        tessitura = "";
        token = "";
    }
}