using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DefaultPlugins : INativePlugins
{
    private Dictionary<string, Func<object[], object>> mDict;

    private object CopyText(object[] args)
    {
        TextEditor te = new TextEditor();
        te.text = (string)args[0];
        te.OnFocus();
        te.Copy();
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
