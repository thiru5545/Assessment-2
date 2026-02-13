using System;
using System.Collections.Generic;
using System.Text;

internal class UserMenu
{
    public void usermenu(int id, UserServices ott, VideoServices ott1, RequestServices req)
    {
        while (true)
        {
            Console.WriteLine("1.VIDEOS 2.REQUEST SUB 3.STATUS 4.LOGOUT");
            int o = int.Parse(Console.ReadLine());
            if (o == 1) ott1.videolist(id);
            else if (o == 2) req.addrequest(id);
            else if (o == 3) req.viewrequest(id);
            else break;
        }
    }
}

