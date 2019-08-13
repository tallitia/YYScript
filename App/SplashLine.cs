using UnityEngine;
using DG.Tweening;

//显示过场动画
public class SplashLine : LineBase
{
    private GameObject m_Go;

    public override void OnEnter()
    {
        base.OnEnter();
        StartLine();
    }

    public void StartLine()
    {
		LogUtil.StartLog("App Splash Start ...");

        GameObject prefab = Resources.Load<GameObject>("SplashView");
        m_Go = Object.Instantiate(prefab, LuaHelper.GetLayerManager().UIRoot);
        m_Go.transform.SetAsLastSibling();
        CanvasGroup groupTrans = Util.Get<CanvasGroup>(m_Go, "TipsGroup");
        groupTrans.alpha = 1f;
        groupTrans.DOFade(1f, 0.5f);
        groupTrans.DOFade(0f, 0.5f).SetDelay(2f).OnComplete(OnAnimOver);
    }

    public void OnAnimOver()
    {
        LogUtil.StartLog("App Splash Finish ...");
        if (m_Go != null)
            Object.Destroy(m_Go);
        Finish();
    }

}