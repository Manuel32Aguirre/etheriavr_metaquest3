using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance { get; private set; }

    [Header("Referencias UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private Action onClosedCallback;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Aseguramos que empiece cerrado
        panel.SetActive(false);
        closeButton.onClick.AddListener(Close);
    }

    public void ShowAlert(string title, string message, bool isSuccess = false, Action onClose = null)
    {
        panel.SetActive(true);
        titleText.text = title;
        titleText.color = isSuccess ? Color.green : Color.red;
        messageText.text = message;
        
        // Guardamos la acción que queremos que pase al cerrar (ej: cambiar de escena)
        onClosedCallback = onClose;
    }

    public void Close()
    {
        panel.SetActive(false);
        // Si hay una acción pendiente (como ir al login), se ejecuta al cerrar
        onClosedCallback?.Invoke();
        onClosedCallback = null;
    }
}