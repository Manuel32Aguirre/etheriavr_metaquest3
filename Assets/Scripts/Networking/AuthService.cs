using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class AuthService : MonoBehaviour
{
    private string RegisterUrl => NetworkConfig.Instance.BaseUrl + "/api/users";
    private string LoginUrl => NetworkConfig.Instance.BaseUrl + "/api/login";
    private string SongsUrl => NetworkConfig.Instance.BaseUrl + "/api/songs/listar";

    public IEnumerator Register(UserCreateRequest data, Action<string> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(RegisterUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorResponse = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorResponse)) errorResponse = "Error de conexión";
                onError?.Invoke(errorResponse);
            }
        }
    }

    public IEnumerator Login(UserLoginRequest data, Action<string> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(LoginUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                string errorResponse = request.downloadHandler.text;
                if (string.IsNullOrEmpty(errorResponse)) errorResponse = "Error de conexión";
                onError?.Invoke(errorResponse);
            }
        }
    }

    

    public IEnumerator GetSongs(Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(SongsUrl))
        {
            // Enviamos el Token que guardamos en el UserSession
            if (UserSession.Instance != null && !string.IsNullOrEmpty(UserSession.Instance.token))
            {
                request.SetRequestHeader("Authorization", "Bearer " + UserSession.Instance.token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                // Si el JSON es una lista [{},{}], lo envolvemos para el Wrapper
                if (json.StartsWith("[")) json = "{\"songs\":" + json + "}";
                onSuccess?.Invoke(json);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
}