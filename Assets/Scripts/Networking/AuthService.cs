using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System; // Para Exception

public class AuthService : MonoBehaviour
{
    public IEnumerator Register(UserCreateRequest data, Action<UserResponse> onSuccess, Action<string> onError)
    {
        // 1. Construcción de URL
        string url = NetworkConfig.Instance.BaseUrl + NetworkConfig.Instance.registerPath;
        Debug.Log($"[AuthService] 🚀 Iniciando registro en: {url}");

        // 2. Preparación de datos
        string json = JsonUtility.ToJson(data);
        Debug.Log($"[AuthService] 📦 Datos a enviar: {json}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 3. Envío
            Debug.Log("[AuthService] 📡 Enviando petición a la red...");
            yield return request.SendWebRequest();

            // 4. Procesamiento de respuesta
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[AuthService] ✅ ¡Éxito! Respuesta del servidor: {request.downloadHandler.text}");
                
                try 
                {
                    UserResponse response = JsonUtility.FromJson<UserResponse>(request.downloadHandler.text);
                    onSuccess?.Invoke(response);
                }
                catch (Exception e) 
                {
                    Debug.LogError($"[AuthService] ❌ Error al procesar JSON de éxito: {e.Message}");
                    onError?.Invoke("Error interno al procesar respuesta del servidor.");
                }
            }
            else
            {
                // Si llegamos aquí, el servidor respondió con error o no hubo conexión
                string errorMsg = request.downloadHandler.text;
                long code = request.responseCode;
                
                Debug.LogError($"[AuthService] ❌ Error detectado. Código HTTP: {code}");
                Debug.LogError($"[AuthService] ❌ Detalles del error: {request.error}");
                Debug.LogError($"[AuthService] ❌ Cuerpo del error: {errorMsg}");

                // Si el código es 0, es muy probable que sea un problema de Firewall o Cleartext (HTTP)
                if (code == 0) {
                    onError?.Invoke("No se pudo contactar al servidor. Revisa el Firewall o la configuración HTTP en Unity.");
                } else {
                    onError?.Invoke(errorMsg);
                }
            }
        }
    }
}