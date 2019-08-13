using System.Threading;
using System.Collections.Generic;
using System;
using System.IO;

public class ThreadEvent
{
    public string Key;
    public List<object> evParams = new List<object>();
}

public class NotiData
{
    public string evName;
    public object evParam;

    public NotiData(string name, object param)
    {
        this.evName = name;
        this.evParam = param;
    }
}
/// <summary>
/// 当前线程管理器，同时只能做一个任务
/// </summary>
public class ThreadManager : Manager
{
    private Thread thread;
    private Action<NotiData> func;

    static readonly object m_lockObject = new object();
    static Queue<ThreadEvent> events = new Queue<ThreadEvent>();

    delegate void ThreadSyncEvent(NotiData data);
    private ThreadSyncEvent m_SyncEvent;

    private void Start()
    {
        m_SyncEvent = OnSyncEvent;
        thread = new Thread(OnUpdate);
        thread.Start();
    }

    /// <summary>
    /// 添加到事件队列
    /// </summary>
    public void AddEvent(ThreadEvent ev, Action<NotiData> func)
    {
        lock (m_lockObject)
        {
            this.func = func;
            events.Enqueue(ev);
        }
    }

    /// <summary>
    /// 通知事件
    /// </summary>
    private void OnSyncEvent(NotiData data)
    {
        if (this.func != null) func(data);  //回调逻辑层
    }

    // Update is called once per frame
    void OnUpdate()
    {
        while (true)
        {
            lock (m_lockObject)
            {
                if (events.Count > 0)
                {
                    ThreadEvent e = events.Dequeue();
                    try
                    {
                        switch (e.Key)
                        {
                            case NotiConst.EXTRACT_FILE:
                                OnExtractFile(e.evParams);
                                break;
                            case NotiConst.EXTRACT_STREAM:
                                OnExtractStream(e.evParams);
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                    }
                }
            }
            Thread.Sleep(1);
        }
    }

    void OnExtractStream(List<object> evParams)
    {
    }

    void OnExtractFile(List<object> evParams)
    {
        var stream = new MemoryStream((byte[])evParams[0], false);
        Zip.UnZipDirectory(stream, evParams[1].ToString(), OnExtractUpdate);
        stream.Close();

        NotiData data = new NotiData(NotiConst.EXTRACT_FINISH, null);
        if (m_SyncEvent != null) m_SyncEvent(data);
    }

    void OnExtractUpdate(float progress)
    {
        NotiData data = new NotiData(NotiConst.EXTRACT_UPDATE, progress);
        if (m_SyncEvent != null) m_SyncEvent(data);
    }

    public void Destory()
    {
        if (thread != null) thread.Abort();
    }

}