using System;
using System.Collections.Generic;

public class EventLib 
{
    public delegate void EventHandler(object data);

    private string mName;
    private event EventHandler mHandler;

    public EventLib(string name)
    {
        mName = name;
    }

    public void Connect(EventHandler handler)
    {
        mHandler += handler;
    }

    public void Disconnect(EventHandler handler)
    {
        mHandler -= handler;
    }

    public void DisconnectAll()
    {
        Delegate[] handlers = mHandler.GetInvocationList();
        for (int i = 0; i < handlers.Length; i++)
        {
            mHandler -= handlers[i] as EventHandler;
        }
    }

    public void Fire(object data)
    {
        mHandler(data);
    }
}
