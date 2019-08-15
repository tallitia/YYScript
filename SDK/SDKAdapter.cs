using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using LuaInterface;
using MiniJSON;
using System.Collections.Generic;
using System;

public class SDKAdapter : MonoBehaviour
{
    // JAVA回调名称，必须同JAVA一致
    private const string GO_NAME = "SDKAdapter";

    // LUA接收器，负责分派消息
    private const string LUA_RECEIVER = "sdk_mgr.message";

    private INativePlugins mPlugins;
    private static SDKAdapter m_Instance;
    public static SDKAdapter GetInstance()
    {
        if (m_Instance == null)
        {
            GameObject go = new GameObject();
            DontDestroyOnLoad(go);
            go.name = GO_NAME;

            m_Instance = go.AddComponent<SDKAdapter>();
            m_Instance.Initialize();
        }
        return m_Instance;
    }

    private void Initialize()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        mPlugins = new DefaultPlugins();
        mPlugins.Initialize();
#elif UNITY_ANDROID
        mPlugins = new AndroidPlugins();
        mPlugins.Initialize();
#elif UNITY_IOS
        mPlugins = new IOSPlugins();
        mPlugins.Initialize();
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            mPlugins.Call("exit");
    }

    /// 安卓回调函数，格式 (key,参数1,参数2,参数3)
    private void OnMessage(string param)
    {
        Util.Log("SDK Message: " + param);
        string[] args = param.Split('|');
        Util.CallLuaFunction<string, string>(LUA_RECEIVER, args[0], args[1]);
    }

    private IEnumerator WebRequest(string url, Dictionary<string, object> dict, Action<bool, string> callback)
    {
        WWWForm data = new WWWForm();
        foreach (var pair in dict)
        {
            data.AddField(pair.Key, pair.Value.ToString());
        }

        UnityWebRequest request = UnityWebRequest.Post(url, data);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Util.Log("Web Request Failed: " + request.error);
            request.Dispose();
            callback(false, request.error);
            yield break;
        }

        Util.Log("Web Request Result: " + request.downloadHandler.text);
        callback(true, request.downloadHandler.text);
    }

    public void Order(string url, string param)
    {
        Util.Log("Request Order: " + url + " \ndata:" + param);
        Dictionary<string, object> dict = (Dictionary<string, object>)Json.Deserialize(param);
        StartCoroutine(WebRequest(url, dict, (succeed, result)=> {
            if (succeed)
            {
                var args = result.Split(',');
                if (args[0] == "0")
                {
                    OnMessage("ORDER_SUCCEED|" + args[1] + "," + dict["product_id"]);
                    return ;
                }
                else
                {
                    OnMessage("ORDER_FAILED|" + result);
                    return ;
                }
            }

            OnMessage("ORDER_FAILED|" + param);
        }));
    }
    
    /// Native tools...
    public void CopyText(string text) 
    {
        //mPlugins.Call("copyText", text);
    }

    public byte[] LoadAsset(string path)
    {
        return mPlugins.Call<byte[]>("loadAsset", path);
    }

    public string LoadAssetText(string path)
    {
        var bytes = LoadAsset(path);
        if (bytes == null) return string.Empty;
        return System.Text.Encoding.Default.GetString(bytes);
    }

        // 复制文本到剪切板
    public void CopyTextToClipboard(string text)
    {
#if UNITY_EDITOR
        TextEditor te = new TextEditor();
        te.text = text;
        te.OnFocus();
        te.Copy();
#elif UNITY_ANDROID
        //CallActivity("copyTextToClipboard", text);
#elif UNITY_IOS
       //_copyToClipboard(text);
#else
        TextEditor te = new TextEditor();
		te.text = text;
        te.OnFocus();
        te.Copy();
#endif
    }

    /// SDK platform
    public void Login()
    {
        mPlugins.Call("login");
    }

    public void Logout()
    {
        mPlugins.Call("logout");
    }

    public void Purchase(string json)
    {
        mPlugins.Call("purchase", json);
    }

    public void Upload(string json, int type)
    {
        mPlugins.Call("upload", json, type);
    }

    /// TalkingData Game
    public void SetTDEvent(string id)
    {
        mPlugins.Call("setTDEvent", id);
    }

    public void SetTDAccount(string uid)
    {
        mPlugins.Call("setTDAccount", uid);
    }

	public void SetTDServer(string val) 
    {
        mPlugins.Call("setTDServer", val);
	}
	
	public void SetTDName(string val) 
    {
        mPlugins.Call("setTDName", val);
	}
	
	public void SetTDLevel(int val) 
    {
        mPlugins.Call("setTDLevel", val);
	}

    public void SetTDReward(string flag, int val)
    {
        mPlugins.Call("setTDReward", flag, val);
    }

    public void SetTDConsume(string flag, int val)
    {
        mPlugins.Call("setTDConsume", flag, val);
    }
}