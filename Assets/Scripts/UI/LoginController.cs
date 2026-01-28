using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LoginController : MonoBehaviour
{
    [Header("UI de Login")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public Button loginButton;

    [Header("Dependencias")]
    public AuthService authService;

    void Start()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);
    }

    private void OnLoginClicked()
    {
        UserLoginRequest requestData = new UserLoginRequest {
            email = emailField.text,
            password = passwordField.text
        };

        StartCoroutine(authService.Login(requestData, 
            (res) => {
                // 1. INYECTAMOS los datos en la sesión global
                UserSession.Instance.SetUser(
                    res.id, 
                    res.username, 
                    res.email, 
                    res.tessitura, 
                    res.access_token
                );

                // 2. Mostramos bienvenida y cambiamos de escena
                AlertManager.Instance.ShowAlert("¡Bienvenido!", $"Hola, {res.username}.", true, () => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene"); 
    });
            },
            (err) => {
                ParseAndShowError(err);
            }
        ));
    }

    private void ParseAndShowError(string jsonError) {
        try {
            var simpleError = JsonUtility.FromJson<FastAPIError>(jsonError);
            if (!string.IsNullOrEmpty(simpleError.detail)) {
                AlertManager.Instance.ShowAlert("Login Fallido", simpleError.detail, false);
                return;
            }

            var validationError = JsonUtility.FromJson<ValidationError>(jsonError);
            if (validationError?.detail != null && validationError.detail.Length > 0) {
                AlertManager.Instance.ShowAlert("Dato no válido", validationError.detail[0].msg, false);
                return;
            }
        } catch {
            AlertManager.Instance.ShowAlert("Error de Red", "No se pudo conectar con el servidor.", false);
        }
    }
}