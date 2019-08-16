using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

public class PatchLine : LineBase
{
    private int RELOAD_COUNT = 3;
    private int m_Count = 0;

    Dictionary<string, string> localVersion = new Dictionary<string, string>();
    Dictionary<string, ulong> localSize = new Dictionary<string, ulong>();
    Dictionary<string, string> assetsVersion = new Dictionary<string, string>();
    Dictionary<string, ulong> assetsSize = new Dictionary<string, ulong>();

    public override bool CheckLine()
    {
        bool ret = true;
        if (!AppConst.CheckVersion)
            ret = false;
        if (AppConst.isWin)
            ret = false;

        return false;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        PatchLineStart();
    }


    public void PatchLineStart()
    {
        CoroutineRun(CheckVersion());
    }

    private IEnumerator CheckVersion()
    {
        UpdateTips("正在检查游戏版本", 1);
        string localVerPath = AppConst.CachePath + AppConst.VerionName;
        if (File.Exists(localVerPath))
            DecodeVersionFile(File.ReadAllText(localVerPath), localVersion, localSize);
        else if (!Directory.Exists(AppConst.CachePath))
            Directory.CreateDirectory(AppConst.CachePath);


        string assetVerPath = AppConst.AssetsPath + AppConst.VerionName;
        DecodeVersionFile( SDKAdapter.GetInstance().LoadAssetText(assetVerPath), assetsVersion, assetsSize);

        //load remote version file
        string remoteVerPath = AppConst.AssetBundleURI + AppConst.VerionName + "?r=" + Util.GetTime();
        LogUtil.StartLog("Check Game Version: " + remoteVerPath);

        string remoteVerContent = null;
        UnityWebRequest request = UnityWebRequest.Get(remoteVerPath);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            UpdateTips("检查游戏版本失败:" + request.error, 0);
            LogUtil.StartLog("Failed To Load Version File: " + remoteVerPath + "\n" + request.error);
            request.Dispose();

            // 3秒后重新开始检查
            yield return new WaitForSeconds(3f);
            ReLoad();
            yield break;
        }
        remoteVerContent = request.downloadHandler.text;
        request.Dispose();

        //read remote version file
        Dictionary<string, string> remoteVersion = new Dictionary<string, string>();
        Dictionary<string, ulong> remoteSize = new Dictionary<string, ulong>();
        DecodeVersionFile(remoteVerContent, remoteVersion, remoteSize);

        //compare version
        List<string> updatefiles = new List<string>();
        CompareVersionFile(localVersion, assetsVersion, remoteVersion, updatefiles);

        //don't need to update
        if (updatefiles.Count == 0)
        {
            yield break;
        }

        //update the file
        ulong totalBytes = 0;
        for (int i = 0; i < updatefiles.Count; i++)
        {
            string abname = updatefiles[i];
            totalBytes += remoteSize[abname];
        }
        Util.Log("Total Download Bytes: " + totalBytes / 1024 + " KB...");

        ulong downloadBytes = 0;
        for (int i = 0; i < updatefiles.Count; i++)
        {
            string fileURL = AppConst.AssetBundleURI + updatefiles[i] + "?r=" + Util.GetTime();
            Util.Log("Update file: " + fileURL);

            UnityWebRequest www = UnityWebRequest.Get(fileURL);
            AsyncOperation op = www.SendWebRequest();
            while (true)
            {
                ulong bytes = downloadBytes + www.downloadedBytes;
                float progress = (float)bytes / totalBytes;
                UpdateTips("正在更新文件", progress);

                if (op.isDone)
                    break;

                yield return null;
            }

            if (www.isNetworkError || www.isHttpError)
            {
                UpdateTips("更新文件失败: " + updatefiles[i], 0);
                Util.Log("Failed to load '" + fileURL + "', update stop.\n" + www.error);
                yield break;
            }

            downloadBytes += www.downloadedBytes;
            string filePath = AppConst.CachePath + updatefiles[i];
            File.WriteAllBytes(filePath, www.downloadHandler.data);
        }

        //update local version file
        File.WriteAllText(localVerPath, remoteVerContent);

    }

    private void DecodeVersionFile(string content, Dictionary<string, string> version, Dictionary<string, ulong>size)
    {
        try
        {
            string[] lines = content.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string[] line = lines[i].Split('|');
                version.Add(line[0], line[1]);
                size.Add(line[0], ulong.Parse(line[2]));
            }
        }catch(Exception e)
        {
            UpdateTips("解析补丁文件失败");
            LogUtil.LogExInfo("解析补丁文件失败", e);
        }
    }

    private void CompareVersionFile(Dictionary<string, string> local, Dictionary<string, string> assets, Dictionary<string, string> remote, List<string> files)
    {
        var assetMgr = MainGame.GetManager<AssetManager>();
        foreach (var pair in remote)
        {
            string abname = pair.Key;
            if (FilterAssetBundle(abname))
                continue;

            string remoteMD5 = pair.Value;
            string localMD5 = null;
            string assetMD5 = null;
            if (assets.TryGetValue(abname, out assetMD5) && remoteMD5.Equals(assetMD5))
                continue;
            else
                assetMgr.AddAssetFlag(abname);

            string localFile = AppConst.CachePath + abname;
            if (!File.Exists(localFile))
            {
                files.Add(abname);
                continue;
            }

            if (local.TryGetValue(abname, out localMD5) && remoteMD5.Equals(localMD5))
                continue; //the same file, compare next one 

            localMD5 = Util.MD5File(localFile);
            if (!remoteMD5.Equals(localMD5))
            {
                //the different file, need to update
                files.Add(abname);
            }
        }
    }

    private bool FilterAssetBundle(string abname)
    {
        string luaMode = AppConst.Is32Bit ? "lua32" : "lua64";
        return abname.StartsWith("lua") && abname != "luaconfig.ab" && !abname.StartsWith(luaMode);
    }

    private void ReLoad()
    {
        m_Count++;
        if (m_Count >= RELOAD_COUNT)
        {
            UpdateTips("加载补丁文件失败", 1);
            LogUtil.StartLog("加载补丁文件失败");
        }
        else
            CoroutineStop();
    }


}
