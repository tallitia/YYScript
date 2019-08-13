using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class IOSPlugins : INativePlugins
{
    [DllImport("__Internal")]
    private static extern void _copyToClipboard(string text);

    private Dictionary<string, Func<object[], object>> mDict;

    private object CopyText(object[] args)
    {
        _copyToClipboard((string)args[0]);
        return null;
    }

    private object LoadAsset(object[] args)
    {
        string path = (string)args[0];
        if (File.Exists(path))
            return File.ReadAllBytes(path);

        return null;
    }

    public void Initialize()
    {
        mDict = new Dictionary<string, Func<object[], object>>();
        mDict.Add("copyText", CopyText);
        mDict.Add("loadAsset", LoadAsset);
    }

    public void Call(string method, params object[] args)
    {
        if (mDict.ContainsKey(method))
            mDict[method].Invoke(args);
    }

    public T Call<T>(string method, params object[] args)
    {
        if (mDict.ContainsKey(method))
            return (T)mDict[method].Invoke(args);

        return default(T);
    }
}
