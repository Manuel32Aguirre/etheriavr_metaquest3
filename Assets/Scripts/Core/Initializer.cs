using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Awake()
    {
        // Al acceder a la propiedad Instance, obligamos a NetworkConfig 
        // a ejecutar su DiscoverServerAsync() de inmediato.
        string url = NetworkConfig.Instance.BaseUrl;
        Debug.Log("<color=cyan>[App] Iniciando búsqueda automática del servidor...</color>");
    }
}