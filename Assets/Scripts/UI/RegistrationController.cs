using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegistrationController : MonoBehaviour
{
    [Header("UI de Registro")]
    public TMP_InputField usernameField;
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public Button registerButton;
    public TMP_Text statusText;

    [Header("Dependencias")]
    public AuthService authService;

    void Start()
    {
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);
    }

    private void OnRegisterClicked()
    {
        // Validación rápida de contraseñas
        if (passwordField.text != confirmPasswordField.text) {
            SetStatus("❌ Las contraseñas no coinciden", Color.red);
            return;
        }

        SetStatus("Registrando en Etheria...", Color.white);

        // Creamos el DTO de petición
        UserCreateRequest requestData = new UserCreateRequest {
            username = usernameField.text,
            email = emailField.text,
            password = passwordField.text,
            confirm_password = confirmPasswordField.text
        };

        // Disparamos la petición
        StartCoroutine(authService.Register(requestData, 
            (res) => SetStatus($"✅ ¡Éxito! ID: {res.id}", Color.green),
            (err) => SetStatus($"❌ Error: {err}", Color.red)
        ));
    }

    private void SetStatus(string msg, Color col) {
        statusText.text = msg;
        statusText.color = col;
    }
}