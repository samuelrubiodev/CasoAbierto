using UnityEngine;

using System;

public class InternetConnectionNotFound : Exception
{
    public InternetConnectionNotFound()
    {
    }

    public InternetConnectionNotFound(string message)
        : base(message)
    {
    }

    public InternetConnectionNotFound(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class InternetConnection 
{

    public void CheckInternetConnection() {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            throw new InternetConnectionNotFound("No internet connection available");
        }
    }
}