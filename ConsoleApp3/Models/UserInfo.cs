using System;
using System.Collections.Generic;
using System.Text;

internal class Userinfo
{
    public int Id;
    public string Username;
    public string Password;
    public string Email;
    public Subscription Sub;
    public Role Role;

    public Userinfo(int id, string user, string pass, string mail, Subscription sub, Role role)
    {
        Id = id;
        Username = user;
        Password = pass;
        Email = mail;
        Sub = sub;
        Role = role;
    }
}

