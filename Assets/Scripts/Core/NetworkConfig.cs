using UnityEngine;

// Quitamos el CreateAssetMenu porque ya no vamos a crear el archivo manualmente
public class NetworkConfig : ScriptableObject
{
    // Acceso global: Cualquier script puede llamar a NetworkConfig.Instance
    public static NetworkConfig Instance { get; private set; }

    [Header("Valores dinámicos (se cargan del .env)")]
    public string ipAddress = "127.0.0.1";
    public string port = "8000";
    public bool useHttps = false;

    [Header("Endpoints")]
    public string registerPath = "/api/users";
    public string loginPath = "/api/login";

    public string BaseUrl => $"{(useHttps ? "https" : "http")}://{ipAddress}:{port}";

    // --- AUTOCARGA PURA ---
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        // En lugar de buscar un archivo, lo creamos en el aire (en memoria)
        Instance = CreateInstance<NetworkConfig>();
        Instance.LoadFromEnv();
    }

    public void LoadFromEnv()
    {
        var env = EnvLoader.Load();
        if (env != null)
        {
            if (env.ContainsKey("SERVER_IP")) ipAddress = env["SERVER_IP"];
            if (env.ContainsKey("SERVER_PORT")) port = env["SERVER_PORT"];
            if (env.ContainsKey("USE_HTTPS")) useHttps = bool.Parse(env["USE_HTTPS"]);
        }
        
        Debug.Log($"🌐 Red configurada globalmente: {BaseUrl}");
    }
}