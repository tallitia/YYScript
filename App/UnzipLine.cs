using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class UnzipLine : LineBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        UnzipLineStart();
    }

    public override bool CheckLine()
    {
        bool ret = true;
        if (AppConst.isWin)
            ret = false;

        return false;
    }

    public void UnzipLineStart()
    {
        LogUtil.StartLog("UnzipLineStart");
        CoroutineRun(ExtractZipFile());
        Finish();
    }

    private IEnumerator ExtractZipFile()
    {
        bool bNeedUnzip = false;
        string version = PlayerPrefs.GetString("Version");

        if (version != AppConst.AppVersion)
            bNeedUnzip = true;
        if (bNeedUnzip == false)
        {
            Finish();
            yield break;
        }

        string path = AppConst.ABZipPath;
        LogUtil.StartLog("Extract Zip File: " + path);
        UpdateTips("正在解压文件");
        byte[] bytes = null;
        if (path.Contains("://"))
        {
            UnityWebRequest request = UnityWebRequest.Get(path);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                LogUtil.StartLog("Failed To Load ab.zip File: " + path + "\n" + request.error);
            else
                bytes = request.downloadHandler.data;
        }
        else if (File.Exists(path))
            bytes = File.ReadAllBytes(path);

        if (bytes != null)
        {
            object[] param = new object[] { bytes, AppConst.CachePath };
            ThreadEvent ev = new ThreadEvent();
            ev.Key = NotiConst.EXTRACT_FILE;
            ev.evParams.AddRange(param);

            float progress = 0f;
            bool isFinish = false;
            ThreadManager threadManager = MainGame.GetManager<ThreadManager>();
            threadManager.AddEvent(ev, (e) =>
            {
                if (e.evName == NotiConst.EXTRACT_UPDATE)
                    progress = (float)e.evParam;
                else if (e.evName == NotiConst.EXTRACT_FINISH)
                    isFinish = true;
            });   //线程解压

            while (!isFinish)
            {
                UpdateTips("正在解压文件", progress);
                yield return new WaitForEndOfFrame();
            }
            PlayerPrefs.SetString("Version", AppConst.AppVersion);
            Util.Log("Extract Zip File Finish.");
            UpdateTips("解压文件完成", 1);
        }
        else
        {
            Util.Log("Zip File Is Not Found.");
            UpdateTips("解压文件失败");
        }
    }

}
