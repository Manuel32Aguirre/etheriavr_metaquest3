using UnityEngine;
using TMPro;

public class UserInfoUI : MonoBehaviour
{
    public TMP_Text userText;

    void Start()
    {
        if (UserSession.Instance == null)
        {
            userText.text = "No hay sesión :(";
            return;
        }

        userText.text =
            $"ID: {UserSession.Instance.id}\n" +
            $"User: {UserSession.Instance.username}\n" +
            $"Email: {UserSession.Instance.email}\n" +
            $"Tessitura: {UserSession.Instance.tessitura}";
    }
}
