using UnityEngine;

public class NetworkConfig : ScriptableObject
{
    private static NetworkConfig _instance;

    // Acceso global con inicialización inteligente (Lazy Loading)
    public static NetworkConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateInstance<NetworkConfig>();
                _instance.LoadFromEnv();
                Debug.Log("[NetworkConfig] 🛠️ Configuración inicializada dinámicamente.");
            }
            return _instance;
        }
    }

    [Header("Valores dinámicos (se cargan del .env)")]
    public string ipAddress = "127.0.0.1";
    public string port = "8000";
    public bool useHttps = false;

    [Header("Endpoints")]
    public string registerPath = "/api/users";
    public string loginPath = "/api/login";

    // Propiedad calculada para la URL base
    public string BaseUrl => $"{(useHttps ? "https" : "http")}://{ipAddress}:{port}";

    public void LoadFromEnv()
    {
        try
        {
            var env = EnvLoader.Load();
            if (env != null && env.Count > 0)
            {
                if (env.ContainsKey("SERVER_IP")) ipAddress = env["SERVER_IP"];
                if (env.ContainsKey("SERVER_PORT")) port = env["SERVER_PORT"];
                if (env.ContainsKey("USE_HTTPS")) useHttps = bool.Parse(env["USE_HTTPS"]);

                Debug.Log($"🌐 Red configurada: {BaseUrl}");
            }
            else
            {
                Debug.LogWarning("⚠️ No se pudo cargar .env o está vacío. Usando valores por defecto.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error al cargar configuración desde .env: {e.Message}");
        }
    }
}