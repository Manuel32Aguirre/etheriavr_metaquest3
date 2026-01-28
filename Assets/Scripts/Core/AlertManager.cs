using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AlertManager : MonoBehaviour
{
    // Campo privado para la instancia
    private static AlertManager _instance;

    // Propiedad pública "Inteligente"
    public static AlertManager Instance
    {
        get
        {
            // Si la referencia se perdió o no se ha asignado...
            if (_instance == null)
            {
                // Buscamos en toda la escena si existe el objeto
                _instance = FindFirstObjectByType<AlertManager>();

                // Si de plano no existe en la jerarquía, avisamos al programador
                if (_instance == null)
                {
                    Debug.LogError("<color=red>[AlertManager] ❌ ERROR CRÍTICO: Se intentó mostrar una alerta pero NO HAY un AlertManager en la escena activa. Arrastra el Prefab a la jerarquía.</color>");
                }
            }
            return _instance;
        }
    }

    [Header("Referencias UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private Action onClosedCallback;

    private void Awake()
    {
        // Validación de Singleton para evitar que existan dos Managers al mismo tiempo
        if (_instance == null)
        {
            _instance = this;
            // DontDestroyOnLoad(gameObject); // Opcional: Actívalo si quieres que este objeto viva entre escenas
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Aseguramos que el panel empiece oculto
        if (panel != null) panel.SetActive(false);
        
        // Configuramos el botón de cerrar una sola vez
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners(); // Evitamos duplicados
            closeButton.onClick.AddListener(Close);
        }
    }

    public void ShowAlert(string title, string message, bool isSuccess = false, Action onClose = null)
    {
        // Protección extra: Si olvidaste arrastrar los textos en el Inspector, no crasheamos
        if (panel == null || titleText == null || messageText == null)
        {
            Debug.LogWarning("[AlertManager] ⚠️ Faltan referencias de UI en el Inspector. Revisa el objeto AlertManager.");
            onClose?.Invoke();
            return;
        }

        panel.SetActive(true);
        titleText.text = title;
        titleText.color = isSuccess ? Color.green : Color.red;
        messageText.text = message;
        
        // Guardamos la acción que queremos que pase al cerrar
        onClosedCallback = onClose;
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        
        // Si hay una acción pendiente (ej: cambiar de escena), se ejecuta ahora
        onClosedCallback?.Invoke();
        onClosedCallback = null;
    }
}