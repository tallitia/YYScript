using UnityEngine;
using System.Collections;

public abstract class LineBase  {

    protected Coroutine coroutine;
    public virtual void Finish()
    {
        MainGame.appView.Run();
    }

    public void UpdateTips(string pMsg, float pValue = 0)
    {
        MainGame.appView.UpdateTips(pMsg, pValue);
    }

    public virtual void OnEnter()
    {
        
    }
    
    public virtual bool CheckLine()
    {
        return true;
    }

    protected void CoroutineRun(IEnumerator pCo)
    {
        MainGame.StopCo(coroutine); //停掉上一个协程
        coroutine = MainGame.StartCo(pCo);
    }
    protected void CoroutineStop()
    {
        MainGame.StopCo(coroutine);
    }
}
