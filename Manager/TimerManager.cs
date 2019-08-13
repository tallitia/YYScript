using System.Collections.Generic;
using System;

public class TimerInfo
{
    public long tick;
    public bool stop;
    public bool delete;
    public Action callback;

    public TimerInfo(Action callback)
    {
        this.callback = callback;
        delete = false;
    }
}

public class TimerManager : Manager
{
    private float interval = 0;
    private List<TimerInfo> objects = new List<TimerInfo>();

    public float Interval
    {
        get { return interval; }
        set { interval = value; }
    }

    private void Start()
    {
        StartTimer(AppConst.TimerInterval);
    }

    /// <summary>
    /// ������ʱ��
    /// </summary>
    public void StartTimer(float value)
    {
        interval = value;
        //MainGame.main.InvokeRepeating("Run", 0, interval);
    }

    /// <summary>
    /// ֹͣ��ʱ��
    /// </summary>
    public void StopTimer()
    {
        //MainGame.main.CancelInvoke("Run");
    }

    /// <summary>
    /// ��Ӽ�ʱ���¼�
    /// </summary>
    public void AddTimerEvent(TimerInfo info)
    {
        if (!objects.Contains(info))
        {
            objects.Add(info);
        }
    }

    /// <summary>
    /// ɾ����ʱ���¼�
    /// </summary>
    public void RemoveTimerEvent(TimerInfo info)
    {
        if (objects.Contains(info) && info != null)
        {
            info.delete = true;
        }
    }

    /// <summary>
    /// ֹͣ��ʱ���¼�
    /// </summary>
    public void StopTimerEvent(TimerInfo info)
    {
        if (objects.Contains(info) && info != null)
        {
            info.stop = true;
        }
    }

    /// <summary>
    /// ������ʱ���¼�
    /// </summary>
    public void ResumeTimerEvent(TimerInfo info)
    {
        if (objects.Contains(info) && info != null)
        {
            info.delete = false;
        }
    }

    /// <summary>
    /// ��ʱ������
    /// </summary>
    void Run()
    {
        if (objects.Count == 0) return;
        for (int i = 0; i < objects.Count; i++)
        {
            TimerInfo o = objects[i];
            if (o.delete || o.stop) { continue; }
            o.callback();
            o.tick++;
        }
        /////////////////////////������Ϊɾ�����¼�///////////////////////////
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            if (objects[i].delete) { objects.Remove(objects[i]); }
        }
    }
}