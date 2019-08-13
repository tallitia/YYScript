using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{
    private TcpClient client = null;
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;

    private const int CONNECT_TIMEOUT = 8;  //连接超时
    private const int MESSAGE_TIMEOUT = 8;  //发送, 接受超时

    private const int MAX_READ = 8192;
    private byte[] byteBuffer = new byte[MAX_READ];

    private TimerManager timerManager;
    private TimerInfo timerInfo;
    // Use this for initialization
    public SocketClient()
    {
    }

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister()
    {
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
        timerManager = MainGame.GetManager<TimerManager>();
        timerInfo = new TimerInfo(OnTimeOut);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        this.Close();
        timerManager = null;
        timerInfo = null;
        if (reader != null)
        {
            reader.Close();
            memStream.Close();
        }
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void Connect(string host, int port)
    {
        client = null;
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                client = new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }

            timerInfo.tick = 0;
            timerInfo.delete = false;
            timerManager.AddTimerEvent(timerInfo);

            client.SendTimeout = MESSAGE_TIMEOUT * 1000;
            client.ReceiveTimeout = MESSAGE_TIMEOUT * 1000;
            client.NoDelay = true;
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch (Exception e)
        {
            Debug.Log("connect failed: " + e.Message);
            OnConnectFailed();
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        outStream = client.GetStream();
        client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
        NetworkManager.AddEvent(Protocal.Connect, null);
    }

    void OnTimeOut()
    {
        if (client != null && client.Connected)
        {
            timerManager.RemoveTimerEvent(timerInfo);
            return;
        }

        if (timerInfo.tick >= CONNECT_TIMEOUT)
        {
            Debug.Log("connect failed: TIMEOUT!");
            OnConnectFailed();
        }
    }

    /// <summary>
    /// 连接失败
    /// </summary>
    void OnConnectFailed()
    {
        Close();
        timerManager.RemoveTimerEvent(timerInfo);
        NetworkManager.AddEvent(Protocal.ConnectFailed, null);
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] message)
    {
		using (MemoryStream ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            ushort msglen = (ushort)message.Length;
			writer.Write(Converter.GetBigEndian(msglen));
            writer.Write(message);
            writer.Flush();
            if (client != null && client.Connected)
            {
                //NetworkStream stream = client.GetStream();
                byte[] payload = ms.ToArray();
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogError("client.connected----->>false");
            }
        }
    }

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        try
        {
            lock (client.GetStream())
            {         //读取字节流到缓冲区
                bytesRead = client.GetStream().EndRead(asr);
            }
            if (bytesRead < 1)
            {                //包尺寸有问题，断线处理
                OnDisconnected(DisType.Disconnect, "bytesRead < 1");
                return;
            }
            OnReceive(byteBuffer, bytesRead);   //分析数据包内容，抛给逻辑层
            lock (client.GetStream())
            {         //分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                client.GetStream().BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), null);
            }
        }
        catch (Exception ex)
        {
            //PrintBytes();
            OnDisconnected(DisType.Exception, ex.Message);
        }
    }

    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Close();   //关掉客户端链接
        int protocal = dis == DisType.Exception ?
        Protocal.Exception : Protocal.Disconnect;

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteShort((ushort)protocal);
        NetworkManager.AddEvent(protocal, buffer);
    }

    /// <summary>
    /// 打印字节
    /// </summary>
    /// <param name="bytes"></param>
    void PrintBytes()
    {
        string returnStr = string.Empty;
        for (int i = 0; i < byteBuffer.Length; i++)
        {
            returnStr += byteBuffer[i].ToString("X2");
        }
        Debug.LogError(returnStr);
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            Debug.LogError("OnWrite--->>>" + ex.Message);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytes() > 2)
        {
		ushort messageLen = Converter.GetBigEndian(reader.ReadUInt16());
            if (RemainingBytes() >= messageLen)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms);
            }
            else
            {
                //Back up the position two bytes
                memStream.Position = memStream.Position - 2;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms)
    {
        BinaryReader r = new BinaryReader(ms);
        byte[] message = r.ReadBytes((int)(ms.Length - ms.Position));

        ByteBuffer buffer = new ByteBuffer(message);
		NetworkManager.AddEvent(Protocal.Message, buffer);
    }


    /// <summary>
    /// 会话发送
    /// </summary>
    void SessionSend(byte[] bytes)
    {
        WriteMessage(bytes);
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (client != null)
        {
            if (client.Connected) client.Close();
            client = null;
        }
    }

    /// <summary>
    /// 发送连接请求
    /// </summary>
	public void SendConnect(string host, int port)
    {
        Connect(host, port);
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    public void SendMessage(ByteBuffer buffer)
    {
        SessionSend(buffer.ToBytes());
        buffer.Close();
    }

	/// <summary>
	/// 是否已经连接
	/// </summary>
	public bool IsConnected()
	{
		return client != null && client.Connected;
	}
}
