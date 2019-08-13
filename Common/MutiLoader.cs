using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MutiLoader {

    private int m_Count = 1;
    private int m_DestCount = 2;
    UnityWebRequest m_Request;
    public string text = "";

    public MutiLoader(int pCounts)
    {
        m_DestCount = pCounts;
    }

    public IEnumerator StartLoadText(string path)
    {
        if (m_Count > m_DestCount)
            yield break;

        m_Request = UnityWebRequest.Get(path);
        yield return m_Request.SendWebRequest();
        if (m_Request.isNetworkError || m_Request.isHttpError)
        {
            LogUtil.Debug(string.Format("Failed To Load: [{0}]次, [Path]{1}, [Error]{2}", m_Count, path, m_Request.error));
            m_Count++;
            yield return new WaitForSeconds(0.5f);
            yield return StartLoadText(path);
            yield break;
        }
        text = m_Request.downloadHandler.text;
        m_Request.Dispose();
        m_Request = null;
    }

}
