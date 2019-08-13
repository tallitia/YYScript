using System;
using UnityEngine;

public class AndroidPlugins : INativePlugins
{
    private static string STREAM_PATH = Application.streamingAssetsPath + "/";
    private AndroidJavaObject mAcitivy;
    public void Initialize()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mAcitivy = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void Call(string method, params object[] args)
    {
        try
        {
            mAcitivy.Call(method, args);
        }
        catch (Exception e)
        {
            Util.Log("Call Native Failed: " + method + "\n" + e.Message);
        }
    }

    public T Call<T>(string method, params object[] args)
    {
        try
        {
            if (method == "loadAsset")
            {
                var path = ((string)args[0]).Replace(STREAM_PATH, "");
                return mAcitivy.Call<T>(method, path);
            }
            return mAcitivy.Call<T>(method, args);
        }
        catch (Exception e)
        {
            Util.Log("Call Native Failed: " + method + "\n" + e.Message);
        }
        return default(T);
    }
}
