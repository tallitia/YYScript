using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : Manager
{
    private SocketClient socket;
    static readonly object m_lockObject = new object();
    private static Queue<KeyValuePair<int, ByteBuffer>> m_Events = new Queue<KeyValuePair<int, ByteBuffer>>();


    public override void OnInit()
    {
        base.OnInit();
        SocketClient.OnRegister();
    }

    SocketClient SocketClient
    {
        get
        {
            if (socket == null)
                socket = new SocketClient();
            return socket;
        }
    }

    public static void AddEvent(int pEvent, ByteBuffer pData)
    {
        lock (m_lockObject)
        {
            m_Events.Enqueue(new KeyValuePair<int, ByteBuffer>(pEvent, pData));
        }
    }

    public void Update()
    {
        if (m_Events.Count > 0)
        {
            while (m_Events.Count > 0)
            {
                KeyValuePair<int, ByteBuffer> _event = m_Events.Dequeue();
                if (_event.Key == Protocal.Message)
                    Util.CallLuaFunction<LuaInterface.LuaByteBuffer>("Network.OnMessage", _event.Value.ReadBuffer());
                else if (_event.Key == Protocal.Connect)
                    Util.CallLuaFunction("Network.OnConnected");
                else if (_event.Key == Protocal.ConnectFailed)
                    Util.CallLuaFunction("Network.OnConnectFailed");
                else if (_event.Key == Protocal.Disconnect)
                    Util.CallLuaFunction("Network.OnDisconnected");
                else if (_event.Key == Protocal.Exception)
                    Util.CallLuaFunction("Network.OnException");
            }
        }
    }

	public void Connect(string host, int port)
    {
        SocketClient.SendConnect(host, port);
    }

    public void Disconnect()
    {
        SocketClient.Close();
    }

    public void SendMessage(ByteBuffer buffer)
    {
        SocketClient.SendMessage(buffer);
    }

	public bool IsConnected()
	{
		return SocketClient.IsConnected ();
	}

    public override void OnRestart()
    {
        base.OnRestart();
        SocketClient.OnRemove();
        socket = null;
        SocketClient.OnRegister();
    }

    private void OnDestroy()
    {
        LogUtil.StartLog("NetworkManager OnDispose");
        SocketClient.OnRemove();
        m_Events.Clear();
    }

}