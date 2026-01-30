using System;

[Serializable]
public class UserLoginResponse
{
    public string access_token;
    public string token_type;
    public int id;
    public string username;
    public string email;
    public string tessitura;
}