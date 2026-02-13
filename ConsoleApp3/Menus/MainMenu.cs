using System;
using System.Collections.Generic;
using System.Text;

internal class MainMenu
{
    public void Menu()
    {
        UserServices ott = new UserServices();
        VideoServices ott1 = new VideoServices();
        RequestServices req = new RequestServices();

        while (true)
        {
            Console.WriteLine("1.CREATE USER\n2.LOGIN\n3.EXIT");
            int c = int.Parse(Console.ReadLine());

            if (c == 1) ott.CreateUser();
            else if (c == 2)
            {
                int id = ott.UserLogin();
                if (id == 999)
                    new AdminMenu().adminmenu(ott, ott1, req);
                else if (id > 0)
                    new UserMenu().usermenu(id, ott, ott1, req);
                else Console.WriteLine("INVALID LOGIN");
            }
            else break;
        }
    }
}

