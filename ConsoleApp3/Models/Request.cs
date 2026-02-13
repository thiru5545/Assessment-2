using System;
using System.Collections.Generic;
using System.Text;

internal class Request
{
    public int Requestid;
    public int Userid;
    public string Message;
    public RequestType requestType;

    public override string ToString()
    {
        return $"{Requestid}\t{Userid}\t{Message}\t{requestType}";
    }
}

