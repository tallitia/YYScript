using System;

public class LogUtil
{
    //调试日志， 不可见的
    public static void Debug(string pMsg)
    {
        UnityEngine.Debug.Log(pMsg);
    }

    public static void Error(string pMsg)
    {
        UnityEngine.Debug.LogError(pMsg);
    }

    public static void LogExInfo(string pMsg, Exception pE)
    {
        UnityEngine.Debug.LogError(pMsg + pE.Message + pE.StackTrace);
    }

    public static void StartLog(string pMsg)
    {
        UnityEngine.Debug.Log("StartLog------->" + pMsg);
    }

}

