using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
        string url = NetworkConfig.Instance.BaseUrl;
        Debug.Log("<color=cyan>[App] Iniciando búsqueda automática del servidor...</color>");
    }
}