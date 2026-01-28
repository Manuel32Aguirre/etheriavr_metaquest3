using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class AuthService : MonoBehaviour
{
    /// <summary>
    /// Envía una petición de registro al servidor.
    /// </summary>
    public IEnumerator Register(UserCreateRequest data, Action<UserResponse> onSuccess, Action<string> onError)
    {
        string url = NetworkConfig.Instance.BaseUrl + NetworkConfig.Instance.registerPath;
        Debug.Log($"[AuthService] 🚀 Iniciando registro en: {url}");

        string json = JsonUtility.ToJson(data);
        yield return StartCoroutine(PostRequest(url, json, (responseJson) => {
            UserResponse res = JsonUtility.FromJson<UserResponse>(responseJson);
            onSuccess?.Invoke(res);
        }, onError));
    }

    /// <summary>
    /// Envía una petición de login al servidor para obtener un JWT.
    /// </summary>
    public IEnumerator Login(UserLoginRequest data, Action<UserLoginResponse> onSuccess, Action<string> onError)
    {
        // Nota: Asegúrate de que NetworkConfig.Instance.loginPath esté definido como "/api/login"
        string url = NetworkConfig.Instance.BaseUrl + NetworkConfig.Instance.loginPath;
        Debug.Log($"[AuthService] 🔑 Iniciando login en: {url}");

        string json = JsonUtility.ToJson(data);
        Debug.Log($"[AuthService] 📦 Datos de login: {json}");

        yield return StartCoroutine(PostRequest(url, json, (responseJson) => {
            try 
            {
                UserLoginResponse res = JsonUtility.FromJson<UserLoginResponse>(responseJson);
                onSuccess?.Invoke(res);
            }
            catch (Exception e) 
            {
                Debug.LogError($"[AuthService] ❌ Error al procesar JSON de Login: {e.Message}");
                onError?.Invoke("Error al procesar los datos de sesión.");
            }
        }, onError));
    }

    /// <summary>
    /// Método genérico para peticiones POST con JSON para evitar repetir código.
    /// </summary>
    private IEnumerator PostRequest(string url, string json, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"[AuthService] ✅ ¡Éxito! Respuesta: {request.downloadHandler.text}");
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorMsg = request.downloadHandler.text;
                long code = request.responseCode;
                
                Debug.LogError($"[AuthService] ❌ Error {code}: {request.error}");
                Debug.LogError($"[AuthService] ❌ Cuerpo: {errorMsg}");

                if (code == 0) {
                    onError?.Invoke("No se pudo contactar al servidor. Revisa tu conexión o el Firewall.");
                } else {
                    // Pasamos el JSON crudo del error para que el Controller use ParseAndShowError
                    onError?.Invoke(errorMsg);
                }
            }
        }
    }
}