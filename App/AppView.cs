using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppView {
    private Queue<LineBase> m_LineQueue;
    private LineBase curLine;
    private ResourceRequest m_Request;
    public GameObject logo;
    private Text m_Text;
    private Text m_SliderText;
    private Slider m_Slider;
    private float curValue;
    private Coroutine coroutine;

    public AppView(){
        m_LineQueue = new Queue<LineBase>();
        m_LineQueue.Enqueue(new SplashLine());
        m_LineQueue.Enqueue(new SettingLine());
        m_LineQueue.Enqueue(new ConfigLine());
        m_LineQueue.Enqueue(new UnzipLine());
        m_LineQueue.Enqueue(new PatchLine());
        LoadLogo();
    }

    public void Run()
    {
        MainGame.StopCo(coroutine);
        coroutine = MainGame.StartCo(Next());
    }

    public void Stop()
    {
        if (logo != null)
            Object.Destroy(logo);

        MainGame.EnterGame();
    }

    private IEnumerator Next()
    {
        if (m_LineQueue.Count == 0)
        {
            Stop();
            yield break;
        }
        yield return null;
        curLine = m_LineQueue.Dequeue();
        if (curLine.CheckLine())
            curLine.OnEnter();
        else
            Run();
    }

    private void LoadComplate(AsyncOperation async)
    {
        if (!async.isDone)
            return;

        GameObject prefab = m_Request.asset as GameObject;
        logo = Util.AddChild(LuaHelper.GetLayerManager().UIRoot, prefab);
        logo.transform.SetAsFirstSibling();
        m_Text = Util.Find(logo.transform, "TipsText").GetComponent<Text>();
        m_Slider = Util.Find(logo.transform, "Slider").GetComponent<Slider>();
        m_SliderText = Util.Find(logo.transform, "Slider/Loadtext").GetComponent<Text>();
        m_Text.text = "";
        m_SliderText.text = "";
    }

    private void LoadLogo()
    {
        if (m_Request != null)
            return;
        if (logo != null)
            return;
        m_Request = Resources.LoadAsync<GameObject>("LogoView");
        m_Request.completed += LoadComplate;
    }

    public void UpdateTips(string pMsg, float pValue)
    {
        m_Text.text = pMsg;
        UpdateProgress(pValue);
    }

    public void UpdateProgress(float pValue)
    {
        if (pValue - curValue <= 0)
            return;
        m_Slider.value = pValue;
        m_SliderText.text = pValue * 100 + "%";
    }

}
