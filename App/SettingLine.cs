using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Security;

public class SettingLine : LineBase
{

    public override bool CheckLine()
    {
        if (GameUtil.IsInit)
            return false;

        return true;
    }

    public override void OnEnter()
    {
        UpdateTips("正在初始化配置...");
        base.OnEnter();
        InitPlatform();
        MainGame.InitManagers(); //Manager初始化，各种耗时的工作。 Awake已经做了简单的初始化
        CoroutineRun(LoadSettingFile());
    }

    public IEnumerator LoadSettingFile()
    {
        string path = AppConst.SettingPath;
        LogUtil.StartLog("Load Setting File...\n" + path);
        string pContent;
        if (path.Contains("://"))
        {
            MutiLoader loader = new MutiLoader(3);
            yield return loader.StartLoadText(path);
            pContent = loader.text;
        }
        else
        {
            try
            {
                pContent = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                LogUtil.LogExInfo("Failed To Load Setting Xml: " + path + "\n", e);
                yield break;
            }
        }
        if (string.IsNullOrEmpty(pContent))
            yield break;

        LogUtil.StartLog("Read Setting File..." + pContent);
        ParseSetting(pContent);
        SDKAdapter.GetInstance().SetTDEvent("load_config");
        Finish();
    }

    private void ParseSetting(string pContent)
    {
        try
        {
            SecurityElement setting = SecurityElement.FromString(pContent);

            AppConst.AppVersion = setting.Attribute("version");
            AppConst.ConfigURI = setting.Attribute("server");
            AppConst.AppID = setting.Attribute("appid");
            AppConst.AppChannel = setting.Attribute("channel");
            AppConst.AppBuild = int.Parse(setting.Attribute("build"));
        }
        catch(Exception e)
        {
            LogUtil.LogExInfo("Read Setting Error !", e);
        }
    }

    public void InitPlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
                AppConst.isWin = true;
                AppConst.isAndroid = false;
                AppConst.isEditor = true;
                AppConst.isIOS = false;
                AppConst.isMobile = false;
                break;
            case RuntimePlatform.Android:
                AppConst.isWin = false;
                AppConst.isAndroid = true;
                AppConst.isEditor = false;
                AppConst.isIOS = false;
                AppConst.isMobile = true;
                break;
            case RuntimePlatform.IPhonePlayer:
                AppConst.isWin = false;
                AppConst.isAndroid = false;
                AppConst.isEditor = false;
                AppConst.isIOS = true;
                AppConst.isMobile = true;
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsEditor:
                AppConst.isWin = true;
                AppConst.isAndroid = false;
                AppConst.isEditor = true;
                AppConst.isIOS = false;
                AppConst.isMobile = false;
                break;
            default:
                break;
        }
    }

    public override void Finish()
    {
        LogUtil.StartLog("Setting Finish");
        base.Finish();
    }
}
