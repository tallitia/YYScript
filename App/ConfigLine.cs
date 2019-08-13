using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Security;

//读取服务上的配置文件
public class ConfigLine : LineBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        CoroutineRun(GetConfigFile());
    }

    private IEnumerator GetConfigFile()
    {
        string path = AppConst.ConfigPath + "?r=" + Util.GetTime();
        LogUtil.StartLog("Load Config File: " + path);
        UpdateTips("加载服务器配置文件...");

        MutiLoader loader = new MutiLoader(3);
        yield return loader.StartLoadText(path);
        string content = loader.text;
        if (string.IsNullOrEmpty(content))
            yield break;

        LogUtil.StartLog("Load Config Finish\n" + content);
        ParseConfig(content);
        CheckForceExit();
    }

    private void CheckForceExit()
    {
        if (AppConst.AppBuild < AppConst.ConfigBuild)
            UpdateTips("你的客户端版本过低, 请更新!");
        else
            Finish();
    }

    private void ParseConfig(string pContent)
    {
        try
        {
            Config.Set(pContent);
            AppConst.IsLogger = Config.Get("LOGGER") != "0";
            AppConst.IsDebug = Config.Get("IS_DEBUG") == "1";
            AppConst.IsVerify = Config.Get("IS_VERIFY") == "1";
            AppConst.AssetURI = Config.Get("ASSET");
            AppConst.ServerFile = Config.Get("PATH_SERVERS_LIST");
            AppConst.LoginURL = Config.Get("DYNAMIC_SERVER");
            AppConst.OrderURL = Config.Get("WEB");
            string build = Config.Get("BUILD");
            if (!string.IsNullOrEmpty(build))
                AppConst.ConfigBuild = Convert.ToInt32(build);
        }
        catch(Exception e)
        {
            LogUtil.LogExInfo("Parse Config Error ", e);
        }
    }

    public override void Finish()
    {
        LogUtil.StartLog("Config Finish");
        base.Finish();
    }
}
