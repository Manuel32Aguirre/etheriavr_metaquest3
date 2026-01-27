using System;
[Serializable]
public class UserCreateRequest
{
    public string username;
    public string email;
    public string password;
    public string confirm_password;
}