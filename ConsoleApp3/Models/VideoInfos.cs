using System;
using System.Collections.Generic;
using System.Text;

internal class videoinfos
{
    private int Videoid;
    private string Videoname;
    private string Videourl;
    private Subscription Sub;

    public videoinfos(int id, string name, string url, Subscription sub)
    {
        Videoid = id;
        Videoname = name;
        Videourl = url;
        Sub = sub;
    }

    public int videoid => Videoid;
    public string videoname => Videoname;
    public string videourl => Videourl;
    public Subscription sub => Sub;

    public override string ToString()
    {
        return $"{Videoid}\t{Videoname}\t{Videourl}\t{sub}";
    }
}

