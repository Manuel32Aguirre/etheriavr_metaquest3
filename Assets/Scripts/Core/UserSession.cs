using UnityEngine;

public class UserSession : MonoBehaviour
{
    public static UserSession Instance { get; private set; }

    [Header("User Data (mock / real)")]
    public int id;
    public string username;
    public string email;
    public string tessitura; // "Tenor", "Soprano", etc.

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetUser(int id, string username, string email, string tessitura)
    {
        this.id = id;
        this.username = username;
        this.email = email;
        this.tessitura = tessitura;
    }

    public void Clear()
    {
        id = 0;
        username = "";
        email = "";
        tessitura = "";
    }
}
