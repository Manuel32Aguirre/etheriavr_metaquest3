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
            $"Usuario: {UserSession.Instance.username}";
    }
}
