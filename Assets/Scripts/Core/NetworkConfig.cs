using UnityEngine;

public class NetworkConfig : ScriptableObject
{
    private static NetworkConfig _instance;
    public static NetworkConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = CreateInstance<NetworkConfig>();
                _instance.LoadFromEnv();
                Debug.Log("<color=cyan>[NetworkConfig]</color> Configuración inicializada dinámicamente.");
            }
            return _instance;
        }
    }

    [Header("Conexión al Servidor")]
    public string ipAddress = "";
    public string port = "";
    public bool useHttps = false;

    public string BaseUrl
    {
        get
        {
            if (string.IsNullOrEmpty(ipAddress) || string.IsNullOrEmpty(port))
            {
                Debug.LogWarning("<color=orange>[NetworkConfig] Atenciòn: La dirección IP o el Puerto están vacíos.</color>");
                return "";
            }
            return $"{(useHttps ? "https" : "http")}://{ipAddress}:{port}";
        }
    }

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

                Debug.Log($"<color=green>[NetworkConfig] Configuración cargada con éxito: {BaseUrl}</color>");
            }
            else
            {
                Debug.LogError("<color=red>[NetworkConfig] CRÍTICO: No se pudo cargar el archivo .env o está vacío.</color>");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>[NetworkConfig] Error al procesar el .env: {e.Message}</color>");
        }
    }
}