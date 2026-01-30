using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Text;

public class AuthService : MonoBehaviour
{
    private string RegisterUrl => NetworkConfig.Instance.BaseUrl + "/api/users";
    private string LoginUrl => NetworkConfig.Instance.BaseUrl + "/api/login";

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
}