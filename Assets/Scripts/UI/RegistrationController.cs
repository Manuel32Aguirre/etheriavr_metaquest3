using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RegistrationController : MonoBehaviour
{
    [Header("UI de Registro")]
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public Button registerButton;

    [Header("Dependencias")]
    public AuthService authService;

    void Start()
    {
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private void OnRegisterClicked()
    {
        if (passwordField.text != confirmPasswordField.text) {
            AlertManager.Instance.ShowAlert("Error", "Las contraseñas no coinciden.", false);
            return;
        }

        UserCreateRequest requestData = new UserCreateRequest {
            username = usernameField.text,
            email = emailField.text,
            password = passwordField.text,
            confirm_password = confirmPasswordField.text
        };

        StartCoroutine(authService.Register(requestData, 
            (res) => {
                // Al registrar con éxito, limpiamos por si había algo viejo
                UserSession.Instance.Clear(); 
                
                AlertManager.Instance.ShowAlert(
                    "¡Éxito!", 
                    "Tu cuenta ha sido creada.", 
                    true, 
                    () => UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene")
                );
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
                AlertManager.Instance.ShowAlert("Registro Fallido", simpleError.detail, false);
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

// Clases de soporte (puedes moverlas a un script aparte llamado NetworkErrors.cs si quieres)
[Serializable] public class FastAPIError { public string detail; }
[Serializable] public class ValidationError { public ValidationErrorItem[] detail; }
[Serializable] public class ValidationErrorItem { public string msg; public string type; }