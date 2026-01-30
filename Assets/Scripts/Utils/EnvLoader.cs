using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public static class EnvLoader
{
    public static Dictionary<string, string> Load()
    {
        var envData = new Dictionary<string, string>();

        // Esta ruta funciona tanto en PC (Editor) como en el visor Quest
        string path = Path.Combine(Application.streamingAssetsPath, ".env");
        string content = "";

        if (Application.platform == RuntimePlatform.Android)
        {
            // En Android/Quest los archivos están comprimidos, hay que "pedirlos" como una web
            var request = UnityWebRequest.Get(path);
            var operation = request.SendWebRequest();

            // Forzamos a que espere a leer el archivo (solo porque es al arranque)
            while (!operation.isDone) { }

            if (request.result == UnityWebRequest.Result.Success)
            {
                content = request.downloadHandler.text;
            }
            else
            {
                Debug.LogError($"Error cargando .env en Quest: {request.error}");
                return envData;
            }
        }
        else
        {
            // En PC (Editor) lo leemos normal
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }
            else
            {
                Debug.LogWarning("No se encontró el .env en Assets/StreamingAssets/");
            }
        }

        // Procesar el texto del archivo
        if (!string.IsNullOrEmpty(content))
        {
            string[] lines = content.Split(new[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    envData[parts[0].Trim()] = parts[1].Trim();
                }
            }
        }

        return envData;
    }
}