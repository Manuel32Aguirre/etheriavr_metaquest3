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
        // Configuramos el listener del botón
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private void OnRegisterClicked()
    {
        // 1. Validación local rápida
        if (passwordField.text != confirmPasswordField.text) {
            AlertManager.Instance.ShowAlert("Error", "Las contraseñas no coinciden.", false);
            return;
        }

        // 2. Usamos TU DTO: UserCreateRequest
        UserCreateRequest requestData = new UserCreateRequest {
            username = usernameField.text,
            email = emailField.text,
            password = passwordField.text,
            confirm_password = confirmPasswordField.text
        };

        // 3. Llamada a la API mediante AuthService
        StartCoroutine(authService.Register(requestData, 
            (res) => {
                // ÉXITO: res es tu UserResponse
                // Mostramos el alert y al cerrarlo nos lleva al Login
                AlertManager.Instance.ShowAlert(
                    "¡Éxito!", 
                    $"Tu cuenta ha sido creada.", 
                    true, 
                    () => UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene")
                );
            },
            (err) => {
                // ERROR: Procesamos el JSON que mandó FastAPI
                ParseAndShowError(err);
            }
        ));
    }

    // Método para desglosar qué tipo de error mandó FastAPI
    private void ParseAndShowError(string jsonError) {
        try {
            // Intentar parsear como error manual (HTTPException 400)
            var simpleError = JsonUtility.FromJson<FastAPIError>(jsonError);
            if (!string.IsNullOrEmpty(simpleError.detail)) {
                AlertManager.Instance.ShowAlert("Registro Fallido", simpleError.detail, false);
                return;
            }

            // Intentar parsear como error de validación (Pydantic 422)
            var validationError = JsonUtility.FromJson<ValidationError>(jsonError);
            if (validationError?.detail != null && validationError.detail.Length > 0) {
                // Mostramos el primer mensaje de error que encontremos
                AlertManager.Instance.ShowAlert("Dato no válido", validationError.detail[0].msg, false);
                return;
            }
        } catch {
            // Si el JSON no se puede leer, es un error general
            AlertManager.Instance.ShowAlert("Error de Red", "No se pudo conectar con el servidor.", false);
        }
    }
}

// --- Clases de soporte para entender los errores del servidor ---
// Estas clases permiten que JsonUtility entienda el formato de FastAPI

[Serializable]
public class FastAPIError 
{ 
    public string detail; 
}

[Serializable]
public class ValidationError 
{ 
    public ValidationErrorItem[] detail; 
}

[Serializable]
public class ValidationErrorItem 
{ 
    public string msg; 
    public string type;
}