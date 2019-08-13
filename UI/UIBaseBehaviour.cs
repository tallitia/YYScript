using UnityEngine;

public class UIBaseBehaviour : MonoBehaviour
{
    protected object mData;
    protected bool mIsStart = false;
    protected bool mInvalidate = false;
    protected virtual void Start()
    {
        mIsStart = true;
        OnValidate();
    }

    protected virtual void OnDestroy()
    {
    }

    protected virtual void CommitData()
    {

    }

    protected virtual void OnValidate()
    {
        if (mInvalidate)
        {
            mInvalidate = false;
            CommitData();
        }
    }

    public virtual void Invalidate()
    {
        mInvalidate = true;
        if (mIsStart) OnValidate();
    }

    public virtual object Data
    {
        set 
        {
            mData = value;
            Invalidate();
        }
        get
        {
            return mData;
        }
    }
}
