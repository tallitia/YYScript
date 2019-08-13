using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.Collections.Generic;

public static class LuaHelper
{
    public static System.Type GetType(string classname)
    {
        Assembly assb = Assembly.GetExecutingAssembly(); 
        System.Type t = null;
        t = assb.GetType(classname); ;
        if (t == null)
        {
            t = assb.GetType(classname);
        }
        return t;
    }

    /// 语言包管理器
    public static LangManager GetLangManager()
    {
        return MainGame.GetManager<LangManager>();
    }

    /// 层级管理器
    public static LayerManager GetLayerManager()
    {
        return MainGame.GetManager<LayerManager>();
    }

    /// 资源管理器
    public static AssetManager GetAssetManager()
    {
        return MainGame.GetManager<AssetManager>();
    }

    /// 网络管理器
    public static NetworkManager GetNetManager()
    {
        return MainGame.GetManager<NetworkManager>();
    }

    /// 音乐管理器
    public static AudioManager GetAudioManager()
    {
        return MainGame.GetManager<AudioManager>();
    }

    public static List<string> GetList()
    {
        return new List<string>();
    }



}