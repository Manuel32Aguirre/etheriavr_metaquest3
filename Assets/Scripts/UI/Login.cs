using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class Login : MonoBehaviour
{
    [Header("UI de Inicio de Sesión")]
    [SerializeField] private TMP_InputField emailField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;

    [Header("Servicios")]
    [SerializeField] private AuthService authService;

    private void Start()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        string email = emailField.text.Trim();
        string password = passwordField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            AlertManager.Instance.ShowAlert("Campos vacíos", "Por favor ingresa tus credenciales.", false);
            return;
        }

        loginButton.interactable = false;

        UserLoginRequest loginData = new UserLoginRequest
        {
            email = email,
            password = password
        };

        StartCoroutine(authService.Login(loginData,
            onSuccess: (jsonResponse) =>
            {
                Debug.Log("<color=green>>>> Login Exitoso!</color>");

                UserLoginResponse res = JsonUtility.FromJson<UserLoginResponse>(jsonResponse);

                if (UserSession.Instance != null)
                {
                    UserSession.Instance.SetSession(res);
                }
                else
                {
                    Debug.LogWarning("<color=orange>[Login] No se encontró UserSession.Instance en la escena.</color>");
                }

                Debug.Log($"<color=cyan>Token JWT guardado: {res.access_token}</color>");

                AlertManager.Instance.ShowAlert(
                    "¡Bienvenido!",
                    $"Hola de nuevo, {res.username}.",
                    true,
                    onClose: () =>
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
                    }
                );
            },
            onError: (errorJson) =>
            {
                AlertManager.Instance.ShowApiError(errorJson, "Login Fallido");

                loginButton.interactable = true;
            }
        ));
    }
}